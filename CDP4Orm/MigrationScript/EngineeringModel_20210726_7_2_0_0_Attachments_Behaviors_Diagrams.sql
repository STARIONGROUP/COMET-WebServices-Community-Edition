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

-- Attachment is contained (composite) by Thing: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."Attachment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."Attachment" ADD CONSTRAINT "Attachment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Attachment_Container" ON "SchemaName_Replace"."Attachment" ("Container");
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SchemaName_Replace', 'SchemaName_Replace');

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
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Attachment', 'SchemaName_Replace');

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

CREATE OR REPLACE VIEW "SchemaName_Replace"."TopContainer_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TopContainer"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."TopContainer_Data"() AS "TopContainer" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."EngineeringModel_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TopContainer"."ValueTypeDictionary" || "EngineeringModel"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModel"."EngineeringModelSetup",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModel_CommonFileStore"."CommonFileStore",'{}'::text[]) AS "CommonFileStore",
	COALESCE("EngineeringModel_LogEntry"."LogEntry",'{}'::text[]) AS "LogEntry",
	COALESCE("EngineeringModel_Iteration"."Iteration",'{}'::text[]) AS "Iteration",
	COALESCE("EngineeringModel_Book"."Book",'{}'::text[]) AS "Book",
	COALESCE("EngineeringModel_GenericNote"."GenericNote",'{}'::text[]) AS "GenericNote",
	COALESCE("EngineeringModel_ModellingAnnotation"."ModellingAnnotation",'{}'::text[]) AS "ModellingAnnotation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."TopContainer_Data"() AS "TopContainer" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" USING ("Iid")
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
  LEFT JOIN (SELECT "CommonFileStore"."Container" AS "Iid", array_agg("CommonFileStore"."Iid"::text) AS "CommonFileStore"
   FROM "SchemaName_Replace"."CommonFileStore_Data"() AS "CommonFileStore"
   JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" ON "CommonFileStore"."Container" = "EngineeringModel"."Iid"
   GROUP BY "CommonFileStore"."Container") AS "EngineeringModel_CommonFileStore" USING ("Iid")
  LEFT JOIN (SELECT "ModelLogEntry"."Container" AS "Iid", array_agg("ModelLogEntry"."Iid"::text) AS "LogEntry"
   FROM "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry"
   JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" ON "ModelLogEntry"."Container" = "EngineeringModel"."Iid"
   GROUP BY "ModelLogEntry"."Container") AS "EngineeringModel_LogEntry" USING ("Iid")
  LEFT JOIN (SELECT "Iteration"."Container" AS "Iid", array_agg("Iteration"."Iid"::text) AS "Iteration"
   FROM "SchemaName_Replace"."Iteration_Data"() AS "Iteration"
   JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" ON "Iteration"."Container" = "EngineeringModel"."Iid"
   GROUP BY "Iteration"."Container") AS "EngineeringModel_Iteration" USING ("Iid")
  LEFT JOIN (SELECT "Book"."Container" AS "Iid", ARRAY[array_agg("Book"."Sequence"::text), array_agg("Book"."Iid"::text)] AS "Book"
   FROM "SchemaName_Replace"."Book_Data"() AS "Book"
   JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" ON "Book"."Container" = "EngineeringModel"."Iid"
   GROUP BY "Book"."Container") AS "EngineeringModel_Book" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataNote"."Container" AS "Iid", array_agg("EngineeringModelDataNote"."Iid"::text) AS "GenericNote"
   FROM "SchemaName_Replace"."EngineeringModelDataNote_Data"() AS "EngineeringModelDataNote"
   JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" ON "EngineeringModelDataNote"."Container" = "EngineeringModel"."Iid"
   GROUP BY "EngineeringModelDataNote"."Container") AS "EngineeringModel_GenericNote" USING ("Iid")
  LEFT JOIN (SELECT "ModellingAnnotationItem"."Container" AS "Iid", array_agg("ModellingAnnotationItem"."Iid"::text) AS "ModellingAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem"
   JOIN "SchemaName_Replace"."EngineeringModel_Data"() AS "EngineeringModel" ON "ModellingAnnotationItem"."Container" = "EngineeringModel"."Iid"
   GROUP BY "ModellingAnnotationItem"."Container") AS "EngineeringModel_ModellingAnnotation" USING ("Iid");

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

CREATE OR REPLACE VIEW "SchemaName_Replace"."CommonFileStore_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "FileStore"."ValueTypeDictionary" || "CommonFileStore"."ValueTypeDictionary" AS "ValueTypeSet",
	"CommonFileStore"."Container",
	NULL::bigint AS "Sequence",
	"FileStore"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("FileStore_Folder"."Folder",'{}'::text[]) AS "Folder",
	COALESCE("FileStore_File"."File",'{}'::text[]) AS "File",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."FileStore_Data"() AS "FileStore" USING ("Iid")
  JOIN "SchemaName_Replace"."CommonFileStore_Data"() AS "CommonFileStore" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."ModelLogEntry_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ModelLogEntry"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModelLogEntry"."Container",
	NULL::bigint AS "Sequence",
	"ModelLogEntry"."Author",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ModelLogEntry_LogEntryChangelogItem"."LogEntryChangelogItem",'{}'::text[]) AS "LogEntryChangelogItem",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModelLogEntry_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("ModelLogEntry_AffectedItemIid"."AffectedItemIid",'{}'::text[]) AS "AffectedItemIid",
	COALESCE("ModelLogEntry_AffectedDomainIid"."AffectedDomainIid",'{}'::text[]) AS "AffectedDomainIid"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModelLogEntry" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModelLogEntry_Category_Data"() AS "ModelLogEntry_Category"
   JOIN "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry" ON "ModelLogEntry" = "Iid"
   GROUP BY "ModelLogEntry") AS "ModelLogEntry_Category" USING ("Iid")
 LEFT JOIN (SELECT "ModelLogEntry" AS "Iid", array_agg("AffectedItemIid"::text) AS "AffectedItemIid"
   FROM "SchemaName_Replace"."ModelLogEntry_AffectedItemIid_Data"() AS "ModelLogEntry_AffectedItemIid"
   JOIN "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry" ON "ModelLogEntry" = "Iid"
   GROUP BY "ModelLogEntry") AS "ModelLogEntry_AffectedItemIid" USING ("Iid")
 LEFT JOIN (SELECT "ModelLogEntry" AS "Iid", array_agg("AffectedDomainIid"::text) AS "AffectedDomainIid"
   FROM "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Data"() AS "ModelLogEntry_AffectedDomainIid"
   JOIN "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry" ON "ModelLogEntry" = "Iid"
   GROUP BY "ModelLogEntry") AS "ModelLogEntry_AffectedDomainIid" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "LogEntryChangelogItem"."Container" AS "Iid", array_agg("LogEntryChangelogItem"."Iid"::text) AS "LogEntryChangelogItem"
   FROM "SchemaName_Replace"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem"
   JOIN "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry" ON "LogEntryChangelogItem"."Container" = "ModelLogEntry"."Iid"
   GROUP BY "LogEntryChangelogItem"."Container") AS "ModelLogEntry_LogEntryChangelogItem" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."LogEntryChangelogItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "LogEntryChangelogItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"LogEntryChangelogItem"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("LogEntryChangelogItem_AffectedReferenceIid"."AffectedReferenceIid",'{}'::text[]) AS "AffectedReferenceIid"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "LogEntryChangelogItem" AS "Iid", array_agg("AffectedReferenceIid"::text) AS "AffectedReferenceIid"
   FROM "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Data"() AS "LogEntryChangelogItem_AffectedReferenceIid"
   JOIN "SchemaName_Replace"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem" ON "LogEntryChangelogItem" = "Iid"
   GROUP BY "LogEntryChangelogItem") AS "LogEntryChangelogItem_AffectedReferenceIid" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Iteration_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Iteration"."ValueTypeDictionary" AS "ValueTypeSet",
	"Iteration"."Container",
	NULL::bigint AS "Sequence",
	"Iteration"."IterationSetup",
	"Iteration"."TopElement",
	"Iteration"."DefaultOption",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Iteration_Option"."Option",'{}'::text[]) AS "Option",
	COALESCE("Iteration_Publication"."Publication",'{}'::text[]) AS "Publication",
	COALESCE("Iteration_PossibleFiniteStateList"."PossibleFiniteStateList",'{}'::text[]) AS "PossibleFiniteStateList",
	COALESCE("Iteration_Element"."Element",'{}'::text[]) AS "Element",
	COALESCE("Iteration_Relationship"."Relationship",'{}'::text[]) AS "Relationship",
	COALESCE("Iteration_ExternalIdentifierMap"."ExternalIdentifierMap",'{}'::text[]) AS "ExternalIdentifierMap",
	COALESCE("Iteration_RequirementsSpecification"."RequirementsSpecification",'{}'::text[]) AS "RequirementsSpecification",
	COALESCE("Iteration_DomainFileStore"."DomainFileStore",'{}'::text[]) AS "DomainFileStore",
	COALESCE("Iteration_ActualFiniteStateList"."ActualFiniteStateList",'{}'::text[]) AS "ActualFiniteStateList",
	COALESCE("Iteration_RuleVerificationList"."RuleVerificationList",'{}'::text[]) AS "RuleVerificationList",
	COALESCE("Iteration_Stakeholder"."Stakeholder",'{}'::text[]) AS "Stakeholder",
	COALESCE("Iteration_Goal"."Goal",'{}'::text[]) AS "Goal",
	COALESCE("Iteration_ValueGroup"."ValueGroup",'{}'::text[]) AS "ValueGroup",
	COALESCE("Iteration_StakeholderValue"."StakeholderValue",'{}'::text[]) AS "StakeholderValue",
	COALESCE("Iteration_StakeholderValueMap"."StakeholderValueMap",'{}'::text[]) AS "StakeholderValueMap",
	COALESCE("Iteration_SharedDiagramStyle"."SharedDiagramStyle",'{}'::text[]) AS "SharedDiagramStyle",
	COALESCE("Iteration_DiagramCanvas"."DiagramCanvas",'{}'::text[]) AS "DiagramCanvas",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "Iteration_REPLACE"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "Iteration_REPLACE"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "Iteration_REPLACE"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Option"."Container" AS "Iid", ARRAY[array_agg("Option"."Sequence"::text), array_agg("Option"."Iid"::text)] AS "Option"
   FROM "Iteration_REPLACE"."Option_Data"() AS "Option"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "Option"."Container" = "Iteration"."Iid"
   GROUP BY "Option"."Container") AS "Iteration_Option" USING ("Iid")
  LEFT JOIN (SELECT "Publication"."Container" AS "Iid", array_agg("Publication"."Iid"::text) AS "Publication"
   FROM "Iteration_REPLACE"."Publication_Data"() AS "Publication"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "Publication"."Container" = "Iteration"."Iid"
   GROUP BY "Publication"."Container") AS "Iteration_Publication" USING ("Iid")
  LEFT JOIN (SELECT "PossibleFiniteStateList"."Container" AS "Iid", array_agg("PossibleFiniteStateList"."Iid"::text) AS "PossibleFiniteStateList"
   FROM "Iteration_REPLACE"."PossibleFiniteStateList_Data"() AS "PossibleFiniteStateList"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "PossibleFiniteStateList"."Container" = "Iteration"."Iid"
   GROUP BY "PossibleFiniteStateList"."Container") AS "Iteration_PossibleFiniteStateList" USING ("Iid")
  LEFT JOIN (SELECT "ElementDefinition"."Container" AS "Iid", array_agg("ElementDefinition"."Iid"::text) AS "Element"
   FROM "Iteration_REPLACE"."ElementDefinition_Data"() AS "ElementDefinition"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "ElementDefinition"."Container" = "Iteration"."Iid"
   GROUP BY "ElementDefinition"."Container") AS "Iteration_Element" USING ("Iid")
  LEFT JOIN (SELECT "Relationship"."Container" AS "Iid", array_agg("Relationship"."Iid"::text) AS "Relationship"
   FROM "Iteration_REPLACE"."Relationship_Data"() AS "Relationship"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "Relationship"."Container" = "Iteration"."Iid"
   GROUP BY "Relationship"."Container") AS "Iteration_Relationship" USING ("Iid")
  LEFT JOIN (SELECT "ExternalIdentifierMap"."Container" AS "Iid", array_agg("ExternalIdentifierMap"."Iid"::text) AS "ExternalIdentifierMap"
   FROM "Iteration_REPLACE"."ExternalIdentifierMap_Data"() AS "ExternalIdentifierMap"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "ExternalIdentifierMap"."Container" = "Iteration"."Iid"
   GROUP BY "ExternalIdentifierMap"."Container") AS "Iteration_ExternalIdentifierMap" USING ("Iid")
  LEFT JOIN (SELECT "RequirementsSpecification"."Container" AS "Iid", array_agg("RequirementsSpecification"."Iid"::text) AS "RequirementsSpecification"
   FROM "Iteration_REPLACE"."RequirementsSpecification_Data"() AS "RequirementsSpecification"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "RequirementsSpecification"."Container" = "Iteration"."Iid"
   GROUP BY "RequirementsSpecification"."Container") AS "Iteration_RequirementsSpecification" USING ("Iid")
  LEFT JOIN (SELECT "DomainFileStore"."Container" AS "Iid", array_agg("DomainFileStore"."Iid"::text) AS "DomainFileStore"
   FROM "Iteration_REPLACE"."DomainFileStore_Data"() AS "DomainFileStore"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "DomainFileStore"."Container" = "Iteration"."Iid"
   GROUP BY "DomainFileStore"."Container") AS "Iteration_DomainFileStore" USING ("Iid")
  LEFT JOIN (SELECT "ActualFiniteStateList"."Container" AS "Iid", array_agg("ActualFiniteStateList"."Iid"::text) AS "ActualFiniteStateList"
   FROM "Iteration_REPLACE"."ActualFiniteStateList_Data"() AS "ActualFiniteStateList"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "ActualFiniteStateList"."Container" = "Iteration"."Iid"
   GROUP BY "ActualFiniteStateList"."Container") AS "Iteration_ActualFiniteStateList" USING ("Iid")
  LEFT JOIN (SELECT "RuleVerificationList"."Container" AS "Iid", array_agg("RuleVerificationList"."Iid"::text) AS "RuleVerificationList"
   FROM "Iteration_REPLACE"."RuleVerificationList_Data"() AS "RuleVerificationList"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "RuleVerificationList"."Container" = "Iteration"."Iid"
   GROUP BY "RuleVerificationList"."Container") AS "Iteration_RuleVerificationList" USING ("Iid")
  LEFT JOIN (SELECT "Stakeholder"."Container" AS "Iid", array_agg("Stakeholder"."Iid"::text) AS "Stakeholder"
   FROM "Iteration_REPLACE"."Stakeholder_Data"() AS "Stakeholder"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "Stakeholder"."Container" = "Iteration"."Iid"
   GROUP BY "Stakeholder"."Container") AS "Iteration_Stakeholder" USING ("Iid")
  LEFT JOIN (SELECT "Goal"."Container" AS "Iid", array_agg("Goal"."Iid"::text) AS "Goal"
   FROM "Iteration_REPLACE"."Goal_Data"() AS "Goal"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "Goal"."Container" = "Iteration"."Iid"
   GROUP BY "Goal"."Container") AS "Iteration_Goal" USING ("Iid")
  LEFT JOIN (SELECT "ValueGroup"."Container" AS "Iid", array_agg("ValueGroup"."Iid"::text) AS "ValueGroup"
   FROM "Iteration_REPLACE"."ValueGroup_Data"() AS "ValueGroup"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "ValueGroup"."Container" = "Iteration"."Iid"
   GROUP BY "ValueGroup"."Container") AS "Iteration_ValueGroup" USING ("Iid")
  LEFT JOIN (SELECT "StakeholderValue"."Container" AS "Iid", array_agg("StakeholderValue"."Iid"::text) AS "StakeholderValue"
   FROM "Iteration_REPLACE"."StakeholderValue_Data"() AS "StakeholderValue"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "StakeholderValue"."Container" = "Iteration"."Iid"
   GROUP BY "StakeholderValue"."Container") AS "Iteration_StakeholderValue" USING ("Iid")
  LEFT JOIN (SELECT "StakeHolderValueMap"."Container" AS "Iid", array_agg("StakeHolderValueMap"."Iid"::text) AS "StakeholderValueMap"
   FROM "Iteration_REPLACE"."StakeHolderValueMap_Data"() AS "StakeHolderValueMap"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "StakeHolderValueMap"."Container" = "Iteration"."Iid"
   GROUP BY "StakeHolderValueMap"."Container") AS "Iteration_StakeholderValueMap" USING ("Iid")
  LEFT JOIN (SELECT "SharedStyle"."Container" AS "Iid", array_agg("SharedStyle"."Iid"::text) AS "SharedDiagramStyle"
   FROM "Iteration_REPLACE"."SharedStyle_Data"() AS "SharedStyle"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "SharedStyle"."Container" = "Iteration"."Iid"
   GROUP BY "SharedStyle"."Container") AS "Iteration_SharedDiagramStyle" USING ("Iid")
  LEFT JOIN (SELECT "DiagramCanvas"."Container" AS "Iid", array_agg("DiagramCanvas"."Iid"::text) AS "DiagramCanvas"
   FROM "Iteration_REPLACE"."DiagramCanvas_Data"() AS "DiagramCanvas"
   JOIN "SchemaName_Replace"."Iteration_Data"() AS "Iteration" ON "DiagramCanvas"."Container" = "Iteration"."Iid"
   GROUP BY "DiagramCanvas"."Container") AS "Iteration_DiagramCanvas" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Book_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Book"."ValueTypeDictionary" AS "ValueTypeSet",
	"Book"."Container",
	"Book"."Sequence",
	"Book"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Book_Section"."Section",'{}'::text[]) AS "Section",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Book_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Book_Data"() AS "Book" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Book" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Book_Category_Data"() AS "Book_Category"
   JOIN "SchemaName_Replace"."Book_Data"() AS "Book" ON "Book" = "Iid"
   GROUP BY "Book") AS "Book_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Section"."Container" AS "Iid", ARRAY[array_agg("Section"."Sequence"::text), array_agg("Section"."Iid"::text)] AS "Section"
   FROM "SchemaName_Replace"."Section_Data"() AS "Section"
   JOIN "SchemaName_Replace"."Book_Data"() AS "Book" ON "Section"."Container" = "Book"."Iid"
   GROUP BY "Section"."Container") AS "Book_Section" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Section_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Section"."ValueTypeDictionary" AS "ValueTypeSet",
	"Section"."Container",
	"Section"."Sequence",
	"Section"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Section_Page"."Page",'{}'::text[]) AS "Page",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Section_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Section_Data"() AS "Section" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Section" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Section_Category_Data"() AS "Section_Category"
   JOIN "SchemaName_Replace"."Section_Data"() AS "Section" ON "Section" = "Iid"
   GROUP BY "Section") AS "Section_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Page"."Container" AS "Iid", ARRAY[array_agg("Page"."Sequence"::text), array_agg("Page"."Iid"::text)] AS "Page"
   FROM "SchemaName_Replace"."Page_Data"() AS "Page"
   JOIN "SchemaName_Replace"."Section_Data"() AS "Section" ON "Page"."Container" = "Section"."Iid"
   GROUP BY "Page"."Container") AS "Section_Page" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Page_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Page"."ValueTypeDictionary" AS "ValueTypeSet",
	"Page"."Container",
	"Page"."Sequence",
	"Page"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Page_Note"."Note",'{}'::text[]) AS "Note",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Page_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Page_Data"() AS "Page" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Page" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Page_Category_Data"() AS "Page_Category"
   JOIN "SchemaName_Replace"."Page_Data"() AS "Page" ON "Page" = "Iid"
   GROUP BY "Page") AS "Page_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Note"."Container" AS "Iid", ARRAY[array_agg("Note"."Sequence"::text), array_agg("Note"."Iid"::text)] AS "Note"
   FROM "SchemaName_Replace"."Note_Data"() AS "Note"
   JOIN "SchemaName_Replace"."Page_Data"() AS "Page" ON "Note"."Container" = "Page"."Iid"
   GROUP BY "Note"."Container") AS "Page_Note" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Note_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Note"."ValueTypeDictionary" AS "ValueTypeSet",
	"Note"."Container",
	"Note"."Sequence",
	"Note"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Note_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Note_Data"() AS "Note" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Note" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Note_Category_Data"() AS "Note_Category"
   JOIN "SchemaName_Replace"."Note_Data"() AS "Note" ON "Note" = "Iid"
   GROUP BY "Note") AS "Note_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."BinaryNote_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Note"."ValueTypeDictionary" || "BinaryNote"."ValueTypeDictionary" AS "ValueTypeSet",
	"Note"."Container",
	"Note"."Sequence",
	"Note"."Owner",
	"BinaryNote"."FileType",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Note_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Note_Data"() AS "Note" USING ("Iid")
  JOIN "SchemaName_Replace"."BinaryNote_Data"() AS "BinaryNote" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Note" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Note_Category_Data"() AS "Note_Category"
   JOIN "SchemaName_Replace"."Note_Data"() AS "Note" ON "Note" = "Iid"
   GROUP BY "Note") AS "Note_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."TextualNote_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Note"."ValueTypeDictionary" || "TextualNote"."ValueTypeDictionary" AS "ValueTypeSet",
	"Note"."Container",
	"Note"."Sequence",
	"Note"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Note_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."Note_Data"() AS "Note" USING ("Iid")
  JOIN "SchemaName_Replace"."TextualNote_Data"() AS "TextualNote" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Note" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."Note_Category_Data"() AS "Note_Category"
   JOIN "SchemaName_Replace"."Note_Data"() AS "Note" ON "Note" = "Iid"
   GROUP BY "Note") AS "Note_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."GenericAnnotation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."EngineeringModelDataAnnotation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
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
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."EngineeringModelDataNote_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "EngineeringModelDataNote"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelDataNote"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataNote_Data"() AS "EngineeringModelDataNote" USING ("Iid")
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
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ThingReference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ThingReference"."ValueTypeDictionary" AS "ValueTypeSet",
	"ThingReference"."ReferencedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ThingReference_Data"() AS "ThingReference" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."ModellingThingReference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ThingReference"."ValueTypeDictionary" || "ModellingThingReference"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingThingReference"."Container",
	NULL::bigint AS "Sequence",
	"ThingReference"."ReferencedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."ThingReference_Data"() AS "ThingReference" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."DiscussionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "DiscussionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiscussionItem"."ReplyTo",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."DiscussionItem_Data"() AS "DiscussionItem" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."EngineeringModelDataDiscussionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "DiscussionItem"."ValueTypeDictionary" || "EngineeringModelDataDiscussionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelDataDiscussionItem"."Container",
	NULL::bigint AS "Sequence",
	"DiscussionItem"."ReplyTo",
	"EngineeringModelDataDiscussionItem"."Author",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."DiscussionItem_Data"() AS "DiscussionItem" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."ModellingAnnotationItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ContractDeviation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ContractDeviation"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ContractDeviation_Data"() AS "ContractDeviation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequestForWaiver_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ContractDeviation"."ValueTypeDictionary" || "RequestForWaiver"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ContractDeviation_Data"() AS "ContractDeviation" USING ("Iid")
  JOIN "SchemaName_Replace"."RequestForWaiver_Data"() AS "RequestForWaiver" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Approval_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "Approval"."ValueTypeDictionary" AS "ValueTypeSet",
	"Approval"."Container",
	NULL::bigint AS "Sequence",
	"Approval"."Author",
	"Approval"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."Approval_Data"() AS "Approval" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."RequestForDeviation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ContractDeviation"."ValueTypeDictionary" || "RequestForDeviation"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ContractDeviation_Data"() AS "ContractDeviation" USING ("Iid")
  JOIN "SchemaName_Replace"."RequestForDeviation_Data"() AS "RequestForDeviation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ChangeRequest_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ContractDeviation"."ValueTypeDictionary" || "ChangeRequest"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ContractDeviation_Data"() AS "ContractDeviation" USING ("Iid")
  JOIN "SchemaName_Replace"."ChangeRequest_Data"() AS "ChangeRequest" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ReviewItemDiscrepancy_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ReviewItemDiscrepancy"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("ReviewItemDiscrepancy_Solution"."Solution",'{}'::text[]) AS "Solution",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ReviewItemDiscrepancy_Data"() AS "ReviewItemDiscrepancy" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid")
  LEFT JOIN (SELECT "Solution"."Container" AS "Iid", array_agg("Solution"."Iid"::text) AS "Solution"
   FROM "SchemaName_Replace"."Solution_Data"() AS "Solution"
   JOIN "SchemaName_Replace"."ReviewItemDiscrepancy_Data"() AS "ReviewItemDiscrepancy" ON "Solution"."Container" = "ReviewItemDiscrepancy"."Iid"
   GROUP BY "Solution"."Container") AS "ReviewItemDiscrepancy_Solution" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."Solution_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "Solution"."ValueTypeDictionary" AS "ValueTypeSet",
	"Solution"."Container",
	NULL::bigint AS "Sequence",
	"Solution"."Author",
	"Solution"."Owner",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."Solution_Data"() AS "Solution" USING ("Iid")
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

CREATE OR REPLACE VIEW "SchemaName_Replace"."ActionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ActionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	"ActionItem"."Actionee",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ActionItem_Data"() AS "ActionItem" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ChangeProposal_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ChangeProposal"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	"ChangeProposal"."ChangeRequest",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ChangeProposal_Data"() AS "ChangeProposal" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

CREATE OR REPLACE VIEW "SchemaName_Replace"."ContractChangeNotice_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "EngineeringModelDataAnnotation"."ValueTypeDictionary" || "ModellingAnnotationItem"."ValueTypeDictionary" || "ContractChangeNotice"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModellingAnnotationItem"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelDataAnnotation"."Author",
	"EngineeringModelDataAnnotation"."PrimaryAnnotatedThing",
	"ModellingAnnotationItem"."Owner",
	"ContractChangeNotice"."ChangeProposal",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EngineeringModelDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("EngineeringModelDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("ModellingAnnotationItem_ApprovedBy"."ApprovedBy",'{}'::text[]) AS "ApprovedBy",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ModellingAnnotationItem_SourceAnnotation"."SourceAnnotation",'{}'::text[]) AS "SourceAnnotation",
	COALESCE("ModellingAnnotationItem_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SchemaName_Replace"."Thing_Data"() AS "Thing"
  JOIN "SchemaName_Replace"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" USING ("Iid")
  JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" USING ("Iid")
  JOIN "SchemaName_Replace"."ContractChangeNotice_Data"() AS "ContractChangeNotice" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SchemaName_Replace"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SchemaName_Replace"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("SourceAnnotation"::text) AS "SourceAnnotation"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_SourceAnnotation_Data"() AS "ModellingAnnotationItem_SourceAnnotation"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_SourceAnnotation" USING ("Iid")
 LEFT JOIN (SELECT "ModellingAnnotationItem" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SchemaName_Replace"."ModellingAnnotationItem_Category_Data"() AS "ModellingAnnotationItem_Category"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "ModellingAnnotationItem" = "Iid"
   GROUP BY "ModellingAnnotationItem") AS "ModellingAnnotationItem_Category" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SchemaName_Replace"."Attachment_Data"() AS "Attachment"
   JOIN "SchemaName_Replace"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ModellingThingReference"."Container" AS "Iid", array_agg("ModellingThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SchemaName_Replace"."ModellingThingReference_Data"() AS "ModellingThingReference"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "ModellingThingReference"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "ModellingThingReference"."Container") AS "EngineeringModelDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelDataDiscussionItem"."Container" AS "Iid", array_agg("EngineeringModelDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SchemaName_Replace"."EngineeringModelDataDiscussionItem_Data"() AS "EngineeringModelDataDiscussionItem"
   JOIN "SchemaName_Replace"."EngineeringModelDataAnnotation_Data"() AS "EngineeringModelDataAnnotation" ON "EngineeringModelDataDiscussionItem"."Container" = "EngineeringModelDataAnnotation"."Iid"
   GROUP BY "EngineeringModelDataDiscussionItem"."Container") AS "EngineeringModelDataAnnotation_Discussion" USING ("Iid")
  LEFT JOIN (SELECT "Approval"."Container" AS "Iid", array_agg("Approval"."Iid"::text) AS "ApprovedBy"
   FROM "SchemaName_Replace"."Approval_Data"() AS "Approval"
   JOIN "SchemaName_Replace"."ModellingAnnotationItem_Data"() AS "ModellingAnnotationItem" ON "Approval"."Container" = "ModellingAnnotationItem"."Iid"
   GROUP BY "Approval"."Container") AS "ModellingAnnotationItem_ApprovedBy" USING ("Iid");

