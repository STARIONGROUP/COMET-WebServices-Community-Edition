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

CREATE OR REPLACE FUNCTION "SiteDirectory"."Attachment_Data" ()
    RETURNS SETOF "SiteDirectory"."Attachment" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Attachment";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Attachment"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Attachment_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SiteDirectory"."Attachment_FileType_Data" ()
    RETURNS SETOF "SiteDirectory"."Attachment_FileType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Attachment_FileType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Attachment","FileType","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Attachment_FileType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Attachment","FileType","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Attachment_FileType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

DROP VIEW IF EXISTS "SiteDirectory"."DefinedThing_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DefinedThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ParticipantRole_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ParticipantRole_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParticipantRole"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParticipantRole"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ParticipantRole_ParticipantPermission"."ParticipantPermission",'{}'::text[]) AS "ParticipantPermission",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParticipantRole_Data"() AS "ParticipantRole" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParticipantPermission"."Container" AS "Iid", array_agg("ParticipantPermission"."Iid"::text) AS "ParticipantPermission"
   FROM "SiteDirectory"."ParticipantPermission_Data"() AS "ParticipantPermission"
   JOIN "SiteDirectory"."ParticipantRole_Data"() AS "ParticipantRole" ON "ParticipantPermission"."Container" = "ParticipantRole"."Iid"
   GROUP BY "ParticipantPermission"."Container") AS "ParticipantRole_ParticipantPermission" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Attachment_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Attachment"."ValueTypeDictionary" AS "ValueTypeSet",
	"Attachment"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Attachment_FileType"."FileType",'{}'::text[]) AS "FileType"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Attachment_Data"() AS "Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Attachment" AS "Iid", array_agg("FileType"::text) AS "FileType"
   FROM "SiteDirectory"."Attachment_FileType_Data"() AS "Attachment_FileType"
   JOIN "SiteDirectory"."Attachment_Data"() AS "Attachment" ON "Attachment" = "Iid"
   GROUP BY "Attachment") AS "Attachment_FileType" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ReferenceDataLibrary_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"ReferenceDataLibrary"."RequiredRdl",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ReferenceDataLibrary_DefinedCategory"."DefinedCategory",'{}'::text[]) AS "DefinedCategory",
	COALESCE("ReferenceDataLibrary_ParameterType"."ParameterType",'{}'::text[]) AS "ParameterType",
	COALESCE("ReferenceDataLibrary_Scale"."Scale",'{}'::text[]) AS "Scale",
	COALESCE("ReferenceDataLibrary_UnitPrefix"."UnitPrefix",'{}'::text[]) AS "UnitPrefix",
	COALESCE("ReferenceDataLibrary_Unit"."Unit",'{}'::text[]) AS "Unit",
	COALESCE("ReferenceDataLibrary_FileType"."FileType",'{}'::text[]) AS "FileType",
	COALESCE("ReferenceDataLibrary_Glossary"."Glossary",'{}'::text[]) AS "Glossary",
	COALESCE("ReferenceDataLibrary_ReferenceSource"."ReferenceSource",'{}'::text[]) AS "ReferenceSource",
	COALESCE("ReferenceDataLibrary_Rule"."Rule",'{}'::text[]) AS "Rule",
	COALESCE("ReferenceDataLibrary_Constant"."Constant",'{}'::text[]) AS "Constant",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ReferenceDataLibrary_BaseQuantityKind"."BaseQuantityKind",'{}'::text[]) AS "BaseQuantityKind",
	COALESCE("ReferenceDataLibrary_BaseUnit"."BaseUnit",'{}'::text[]) AS "BaseUnit"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceDataLibrary" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("BaseQuantityKind"::text)] AS "BaseQuantityKind"
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Data"() AS "ReferenceDataLibrary_BaseQuantityKind"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceDataLibrary" = "Iid"
   GROUP BY "ReferenceDataLibrary") AS "ReferenceDataLibrary_BaseQuantityKind" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceDataLibrary" AS "Iid", array_agg("BaseUnit"::text) AS "BaseUnit"
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Data"() AS "ReferenceDataLibrary_BaseUnit"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceDataLibrary" = "Iid"
   GROUP BY "ReferenceDataLibrary") AS "ReferenceDataLibrary_BaseUnit" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Category"."Container" AS "Iid", array_agg("Category"."Iid"::text) AS "DefinedCategory"
   FROM "SiteDirectory"."Category_Data"() AS "Category"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Category"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Category"."Container") AS "ReferenceDataLibrary_DefinedCategory" USING ("Iid")
  LEFT JOIN (SELECT "ParameterType"."Container" AS "Iid", array_agg("ParameterType"."Iid"::text) AS "ParameterType"
   FROM "SiteDirectory"."ParameterType_Data"() AS "ParameterType"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ParameterType"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "ParameterType"."Container") AS "ReferenceDataLibrary_ParameterType" USING ("Iid")
  LEFT JOIN (SELECT "MeasurementScale"."Container" AS "Iid", array_agg("MeasurementScale"."Iid"::text) AS "Scale"
   FROM "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "MeasurementScale"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "MeasurementScale"."Container") AS "ReferenceDataLibrary_Scale" USING ("Iid")
  LEFT JOIN (SELECT "UnitPrefix"."Container" AS "Iid", array_agg("UnitPrefix"."Iid"::text) AS "UnitPrefix"
   FROM "SiteDirectory"."UnitPrefix_Data"() AS "UnitPrefix"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "UnitPrefix"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "UnitPrefix"."Container") AS "ReferenceDataLibrary_UnitPrefix" USING ("Iid")
  LEFT JOIN (SELECT "MeasurementUnit"."Container" AS "Iid", array_agg("MeasurementUnit"."Iid"::text) AS "Unit"
   FROM "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "MeasurementUnit"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "MeasurementUnit"."Container") AS "ReferenceDataLibrary_Unit" USING ("Iid")
  LEFT JOIN (SELECT "FileType"."Container" AS "Iid", array_agg("FileType"."Iid"::text) AS "FileType"
   FROM "SiteDirectory"."FileType_Data"() AS "FileType"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "FileType"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "FileType"."Container") AS "ReferenceDataLibrary_FileType" USING ("Iid")
  LEFT JOIN (SELECT "Glossary"."Container" AS "Iid", array_agg("Glossary"."Iid"::text) AS "Glossary"
   FROM "SiteDirectory"."Glossary_Data"() AS "Glossary"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Glossary"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Glossary"."Container") AS "ReferenceDataLibrary_Glossary" USING ("Iid")
  LEFT JOIN (SELECT "ReferenceSource"."Container" AS "Iid", array_agg("ReferenceSource"."Iid"::text) AS "ReferenceSource"
   FROM "SiteDirectory"."ReferenceSource_Data"() AS "ReferenceSource"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceSource"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "ReferenceSource"."Container") AS "ReferenceDataLibrary_ReferenceSource" USING ("Iid")
  LEFT JOIN (SELECT "Rule"."Container" AS "Iid", array_agg("Rule"."Iid"::text) AS "Rule"
   FROM "SiteDirectory"."Rule_Data"() AS "Rule"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Rule"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Rule"."Container") AS "ReferenceDataLibrary_Rule" USING ("Iid")
  LEFT JOIN (SELECT "Constant"."Container" AS "Iid", array_agg("Constant"."Iid"::text) AS "Constant"
   FROM "SiteDirectory"."Constant_Data"() AS "Constant"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Constant"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Constant"."Container") AS "ReferenceDataLibrary_Constant" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."SiteReferenceDataLibrary_View";

CREATE OR REPLACE VIEW "SiteDirectory"."SiteReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" || "SiteReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteReferenceDataLibrary"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceDataLibrary"."RequiredRdl",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ReferenceDataLibrary_DefinedCategory"."DefinedCategory",'{}'::text[]) AS "DefinedCategory",
	COALESCE("ReferenceDataLibrary_ParameterType"."ParameterType",'{}'::text[]) AS "ParameterType",
	COALESCE("ReferenceDataLibrary_Scale"."Scale",'{}'::text[]) AS "Scale",
	COALESCE("ReferenceDataLibrary_UnitPrefix"."UnitPrefix",'{}'::text[]) AS "UnitPrefix",
	COALESCE("ReferenceDataLibrary_Unit"."Unit",'{}'::text[]) AS "Unit",
	COALESCE("ReferenceDataLibrary_FileType"."FileType",'{}'::text[]) AS "FileType",
	COALESCE("ReferenceDataLibrary_Glossary"."Glossary",'{}'::text[]) AS "Glossary",
	COALESCE("ReferenceDataLibrary_ReferenceSource"."ReferenceSource",'{}'::text[]) AS "ReferenceSource",
	COALESCE("ReferenceDataLibrary_Rule"."Rule",'{}'::text[]) AS "Rule",
	COALESCE("ReferenceDataLibrary_Constant"."Constant",'{}'::text[]) AS "Constant",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ReferenceDataLibrary_BaseQuantityKind"."BaseQuantityKind",'{}'::text[]) AS "BaseQuantityKind",
	COALESCE("ReferenceDataLibrary_BaseUnit"."BaseUnit",'{}'::text[]) AS "BaseUnit"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" USING ("Iid")
  JOIN "SiteDirectory"."SiteReferenceDataLibrary_Data"() AS "SiteReferenceDataLibrary" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceDataLibrary" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("BaseQuantityKind"::text)] AS "BaseQuantityKind"
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Data"() AS "ReferenceDataLibrary_BaseQuantityKind"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceDataLibrary" = "Iid"
   GROUP BY "ReferenceDataLibrary") AS "ReferenceDataLibrary_BaseQuantityKind" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceDataLibrary" AS "Iid", array_agg("BaseUnit"::text) AS "BaseUnit"
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Data"() AS "ReferenceDataLibrary_BaseUnit"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceDataLibrary" = "Iid"
   GROUP BY "ReferenceDataLibrary") AS "ReferenceDataLibrary_BaseUnit" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Category"."Container" AS "Iid", array_agg("Category"."Iid"::text) AS "DefinedCategory"
   FROM "SiteDirectory"."Category_Data"() AS "Category"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Category"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Category"."Container") AS "ReferenceDataLibrary_DefinedCategory" USING ("Iid")
  LEFT JOIN (SELECT "ParameterType"."Container" AS "Iid", array_agg("ParameterType"."Iid"::text) AS "ParameterType"
   FROM "SiteDirectory"."ParameterType_Data"() AS "ParameterType"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ParameterType"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "ParameterType"."Container") AS "ReferenceDataLibrary_ParameterType" USING ("Iid")
  LEFT JOIN (SELECT "MeasurementScale"."Container" AS "Iid", array_agg("MeasurementScale"."Iid"::text) AS "Scale"
   FROM "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "MeasurementScale"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "MeasurementScale"."Container") AS "ReferenceDataLibrary_Scale" USING ("Iid")
  LEFT JOIN (SELECT "UnitPrefix"."Container" AS "Iid", array_agg("UnitPrefix"."Iid"::text) AS "UnitPrefix"
   FROM "SiteDirectory"."UnitPrefix_Data"() AS "UnitPrefix"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "UnitPrefix"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "UnitPrefix"."Container") AS "ReferenceDataLibrary_UnitPrefix" USING ("Iid")
  LEFT JOIN (SELECT "MeasurementUnit"."Container" AS "Iid", array_agg("MeasurementUnit"."Iid"::text) AS "Unit"
   FROM "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "MeasurementUnit"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "MeasurementUnit"."Container") AS "ReferenceDataLibrary_Unit" USING ("Iid")
  LEFT JOIN (SELECT "FileType"."Container" AS "Iid", array_agg("FileType"."Iid"::text) AS "FileType"
   FROM "SiteDirectory"."FileType_Data"() AS "FileType"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "FileType"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "FileType"."Container") AS "ReferenceDataLibrary_FileType" USING ("Iid")
  LEFT JOIN (SELECT "Glossary"."Container" AS "Iid", array_agg("Glossary"."Iid"::text) AS "Glossary"
   FROM "SiteDirectory"."Glossary_Data"() AS "Glossary"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Glossary"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Glossary"."Container") AS "ReferenceDataLibrary_Glossary" USING ("Iid")
  LEFT JOIN (SELECT "ReferenceSource"."Container" AS "Iid", array_agg("ReferenceSource"."Iid"::text) AS "ReferenceSource"
   FROM "SiteDirectory"."ReferenceSource_Data"() AS "ReferenceSource"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceSource"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "ReferenceSource"."Container") AS "ReferenceDataLibrary_ReferenceSource" USING ("Iid")
  LEFT JOIN (SELECT "Rule"."Container" AS "Iid", array_agg("Rule"."Iid"::text) AS "Rule"
   FROM "SiteDirectory"."Rule_Data"() AS "Rule"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Rule"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Rule"."Container") AS "ReferenceDataLibrary_Rule" USING ("Iid")
  LEFT JOIN (SELECT "Constant"."Container" AS "Iid", array_agg("Constant"."Iid"::text) AS "Constant"
   FROM "SiteDirectory"."Constant_Data"() AS "Constant"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Constant"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Constant"."Container") AS "ReferenceDataLibrary_Constant" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."Category_View";

CREATE OR REPLACE VIEW "SiteDirectory"."Category_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Category"."ValueTypeDictionary" AS "ValueTypeSet",
	"Category"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Category_SuperCategory"."SuperCategory",'{}'::text[]) AS "SuperCategory",
	COALESCE("Category_PermissibleClass"."PermissibleClass",'{}'::text[]) AS "PermissibleClass"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Category_Data"() AS "Category" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Category" AS "Iid", array_agg("SuperCategory"::text) AS "SuperCategory"
   FROM "SiteDirectory"."Category_SuperCategory_Data"() AS "Category_SuperCategory"
   JOIN "SiteDirectory"."Category_Data"() AS "Category" ON "Category" = "Iid"
   GROUP BY "Category") AS "Category_SuperCategory" USING ("Iid")
 LEFT JOIN (SELECT "Category" AS "Iid", array_agg("PermissibleClass"::text) AS "PermissibleClass"
   FROM "SiteDirectory"."Category_PermissibleClass_Data"() AS "Category_PermissibleClass"
   JOIN "SiteDirectory"."Category_Data"() AS "Category" ON "Category" = "Iid"
   GROUP BY "Category") AS "Category_PermissibleClass" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."CompoundParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."CompoundParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "CompoundParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("CompoundParameterType_Component"."Component",'{}'::text[]) AS "Component",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."CompoundParameterType_Data"() AS "CompoundParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterTypeComponent"."Container" AS "Iid", ARRAY[array_agg("ParameterTypeComponent"."Sequence"::text), array_agg("ParameterTypeComponent"."Iid"::text)] AS "Component"
   FROM "SiteDirectory"."ParameterTypeComponent_Data"() AS "ParameterTypeComponent"
   JOIN "SiteDirectory"."CompoundParameterType_Data"() AS "CompoundParameterType" ON "ParameterTypeComponent"."Container" = "CompoundParameterType"."Iid"
   GROUP BY "ParameterTypeComponent"."Container") AS "CompoundParameterType_Component" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ArrayParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ArrayParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "CompoundParameterType"."ValueTypeDictionary" || "ArrayParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("CompoundParameterType_Component"."Component",'{}'::text[]) AS "Component",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("ArrayParameterType_Dimension"."Dimension",'{}'::text[]) AS "Dimension"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."CompoundParameterType_Data"() AS "CompoundParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ArrayParameterType_Data"() AS "ArrayParameterType" USING ("Iid")
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
 LEFT JOIN (SELECT "ArrayParameterType" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("Dimension"::text)] AS "Dimension"
   FROM "SiteDirectory"."ArrayParameterType_Dimension_Data"() AS "ArrayParameterType_Dimension"
   JOIN "SiteDirectory"."ArrayParameterType_Data"() AS "ArrayParameterType" ON "ArrayParameterType" = "Iid"
   GROUP BY "ArrayParameterType") AS "ArrayParameterType_Dimension" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ParameterTypeComponent"."Container" AS "Iid", ARRAY[array_agg("ParameterTypeComponent"."Sequence"::text), array_agg("ParameterTypeComponent"."Iid"::text)] AS "Component"
   FROM "SiteDirectory"."ParameterTypeComponent_Data"() AS "ParameterTypeComponent"
   JOIN "SiteDirectory"."CompoundParameterType_Data"() AS "CompoundParameterType" ON "ParameterTypeComponent"."Container" = "CompoundParameterType"."Iid"
   GROUP BY "ParameterTypeComponent"."Container") AS "CompoundParameterType_Component" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ScalarParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ScalarParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."EnumerationParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."EnumerationParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "EnumerationParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("EnumerationParameterType_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."EnumerationParameterType_Data"() AS "EnumerationParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "EnumerationValueDefinition"."Container" AS "Iid", ARRAY[array_agg("EnumerationValueDefinition"."Sequence"::text), array_agg("EnumerationValueDefinition"."Iid"::text)] AS "ValueDefinition"
   FROM "SiteDirectory"."EnumerationValueDefinition_Data"() AS "EnumerationValueDefinition"
   JOIN "SiteDirectory"."EnumerationParameterType_Data"() AS "EnumerationParameterType" ON "EnumerationValueDefinition"."Container" = "EnumerationParameterType"."Iid"
   GROUP BY "EnumerationValueDefinition"."Container") AS "EnumerationParameterType_ValueDefinition" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."EnumerationValueDefinition_View";

CREATE OR REPLACE VIEW "SiteDirectory"."EnumerationValueDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EnumerationValueDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"EnumerationValueDefinition"."Container",
	"EnumerationValueDefinition"."Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."EnumerationValueDefinition_Data"() AS "EnumerationValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."BooleanParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."BooleanParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "BooleanParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."BooleanParameterType_Data"() AS "BooleanParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DateParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DateParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "DateParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."DateParameterType_Data"() AS "DateParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."TextParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."TextParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "TextParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."TextParameterType_Data"() AS "TextParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DateTimeParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DateTimeParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "DateTimeParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."DateTimeParameterType_Data"() AS "DateTimeParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."TimeOfDayParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."TimeOfDayParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "TimeOfDayParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."TimeOfDayParameterType_Data"() AS "TimeOfDayParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."QuantityKind_View";

CREATE OR REPLACE VIEW "SiteDirectory"."QuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("QuantityKind_PossibleScale"."PossibleScale",'{}'::text[]) AS "PossibleScale"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" USING ("Iid")
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
 LEFT JOIN (SELECT "QuantityKind" AS "Iid", array_agg("PossibleScale"::text) AS "PossibleScale"
   FROM "SiteDirectory"."QuantityKind_PossibleScale_Data"() AS "QuantityKind_PossibleScale"
   JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" ON "QuantityKind" = "Iid"
   GROUP BY "QuantityKind") AS "QuantityKind_PossibleScale" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."SpecializedQuantityKind_View";

CREATE OR REPLACE VIEW "SiteDirectory"."SpecializedQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "SpecializedQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	"SpecializedQuantityKind"."General",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("QuantityKind_PossibleScale"."PossibleScale",'{}'::text[]) AS "PossibleScale"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" USING ("Iid")
  JOIN "SiteDirectory"."SpecializedQuantityKind_Data"() AS "SpecializedQuantityKind" USING ("Iid")
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
 LEFT JOIN (SELECT "QuantityKind" AS "Iid", array_agg("PossibleScale"::text) AS "PossibleScale"
   FROM "SiteDirectory"."QuantityKind_PossibleScale_Data"() AS "QuantityKind_PossibleScale"
   JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" ON "QuantityKind" = "Iid"
   GROUP BY "QuantityKind") AS "QuantityKind_PossibleScale" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."SimpleQuantityKind_View";

CREATE OR REPLACE VIEW "SiteDirectory"."SimpleQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "SimpleQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("QuantityKind_PossibleScale"."PossibleScale",'{}'::text[]) AS "PossibleScale"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" USING ("Iid")
  JOIN "SiteDirectory"."SimpleQuantityKind_Data"() AS "SimpleQuantityKind" USING ("Iid")
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
 LEFT JOIN (SELECT "QuantityKind" AS "Iid", array_agg("PossibleScale"::text) AS "PossibleScale"
   FROM "SiteDirectory"."QuantityKind_PossibleScale_Data"() AS "QuantityKind_PossibleScale"
   JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" ON "QuantityKind" = "Iid"
   GROUP BY "QuantityKind") AS "QuantityKind_PossibleScale" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DerivedQuantityKind_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DerivedQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "DerivedQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DerivedQuantityKind_QuantityKindFactor"."QuantityKindFactor",'{}'::text[]) AS "QuantityKindFactor",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterType_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("QuantityKind_PossibleScale"."PossibleScale",'{}'::text[]) AS "PossibleScale"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ParameterType_Data"() AS "ParameterType" USING ("Iid")
  JOIN "SiteDirectory"."ScalarParameterType_Data"() AS "ScalarParameterType" USING ("Iid")
  JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" USING ("Iid")
  JOIN "SiteDirectory"."DerivedQuantityKind_Data"() AS "DerivedQuantityKind" USING ("Iid")
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
 LEFT JOIN (SELECT "QuantityKind" AS "Iid", array_agg("PossibleScale"::text) AS "PossibleScale"
   FROM "SiteDirectory"."QuantityKind_PossibleScale_Data"() AS "QuantityKind_PossibleScale"
   JOIN "SiteDirectory"."QuantityKind_Data"() AS "QuantityKind" ON "QuantityKind" = "Iid"
   GROUP BY "QuantityKind") AS "QuantityKind_PossibleScale" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "QuantityKindFactor"."Container" AS "Iid", ARRAY[array_agg("QuantityKindFactor"."Sequence"::text), array_agg("QuantityKindFactor"."Iid"::text)] AS "QuantityKindFactor"
   FROM "SiteDirectory"."QuantityKindFactor_Data"() AS "QuantityKindFactor"
   JOIN "SiteDirectory"."DerivedQuantityKind_Data"() AS "DerivedQuantityKind" ON "QuantityKindFactor"."Container" = "DerivedQuantityKind"."Iid"
   GROUP BY "QuantityKindFactor"."Container") AS "DerivedQuantityKind_QuantityKindFactor" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."SampledFunctionParameterType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."SampledFunctionParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "SampledFunctionParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "IndependentParameterTypeAssignment"."Container" AS "Iid", ARRAY[array_agg("IndependentParameterTypeAssignment"."Sequence"::text), array_agg("IndependentParameterTypeAssignment"."Iid"::text)] AS "IndependentParameterType"
   FROM "SiteDirectory"."IndependentParameterTypeAssignment_Data"() AS "IndependentParameterTypeAssignment"
   JOIN "SiteDirectory"."SampledFunctionParameterType_Data"() AS "SampledFunctionParameterType" ON "IndependentParameterTypeAssignment"."Container" = "SampledFunctionParameterType"."Iid"
   GROUP BY "IndependentParameterTypeAssignment"."Container") AS "SampledFunctionParameterType_IndependentParameterType" USING ("Iid")
  LEFT JOIN (SELECT "DependentParameterTypeAssignment"."Container" AS "Iid", ARRAY[array_agg("DependentParameterTypeAssignment"."Sequence"::text), array_agg("DependentParameterTypeAssignment"."Iid"::text)] AS "DependentParameterType"
   FROM "SiteDirectory"."DependentParameterTypeAssignment_Data"() AS "DependentParameterTypeAssignment"
   JOIN "SiteDirectory"."SampledFunctionParameterType_Data"() AS "SampledFunctionParameterType" ON "DependentParameterTypeAssignment"."Container" = "SampledFunctionParameterType"."Iid"
   GROUP BY "DependentParameterTypeAssignment"."Container") AS "SampledFunctionParameterType_DependentParameterType" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."MeasurementScale_View";

CREATE OR REPLACE VIEW "SiteDirectory"."MeasurementScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("MeasurementScale_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("MeasurementScale_MappingToReferenceScale"."MappingToReferenceScale",'{}'::text[]) AS "MappingToReferenceScale",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."OrdinalScale_View";

CREATE OR REPLACE VIEW "SiteDirectory"."OrdinalScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "OrdinalScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("MeasurementScale_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("MeasurementScale_MappingToReferenceScale"."MappingToReferenceScale",'{}'::text[]) AS "MappingToReferenceScale",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" USING ("Iid")
  JOIN "SiteDirectory"."OrdinalScale_Data"() AS "OrdinalScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ScaleValueDefinition_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ScaleValueDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ScaleValueDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"ScaleValueDefinition"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."RatioScale_View";

CREATE OR REPLACE VIEW "SiteDirectory"."RatioScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "RatioScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("MeasurementScale_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("MeasurementScale_MappingToReferenceScale"."MappingToReferenceScale",'{}'::text[]) AS "MappingToReferenceScale",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" USING ("Iid")
  JOIN "SiteDirectory"."RatioScale_Data"() AS "RatioScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."CyclicRatioScale_View";

CREATE OR REPLACE VIEW "SiteDirectory"."CyclicRatioScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "RatioScale"."ValueTypeDictionary" || "CyclicRatioScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("MeasurementScale_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("MeasurementScale_MappingToReferenceScale"."MappingToReferenceScale",'{}'::text[]) AS "MappingToReferenceScale",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" USING ("Iid")
  JOIN "SiteDirectory"."RatioScale_Data"() AS "RatioScale" USING ("Iid")
  JOIN "SiteDirectory"."CyclicRatioScale_Data"() AS "CyclicRatioScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."IntervalScale_View";

CREATE OR REPLACE VIEW "SiteDirectory"."IntervalScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "IntervalScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("MeasurementScale_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("MeasurementScale_MappingToReferenceScale"."MappingToReferenceScale",'{}'::text[]) AS "MappingToReferenceScale",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" USING ("Iid")
  JOIN "SiteDirectory"."IntervalScale_Data"() AS "IntervalScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."LogarithmicScale_View";

CREATE OR REPLACE VIEW "SiteDirectory"."LogarithmicScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "LogarithmicScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	"LogarithmicScale"."ReferenceQuantityKind",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("MeasurementScale_ValueDefinition"."ValueDefinition",'{}'::text[]) AS "ValueDefinition",
	COALESCE("MeasurementScale_MappingToReferenceScale"."MappingToReferenceScale",'{}'::text[]) AS "MappingToReferenceScale",
	COALESCE("LogarithmicScale_ReferenceQuantityValue"."ReferenceQuantityValue",'{}'::text[]) AS "ReferenceQuantityValue",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" USING ("Iid")
  JOIN "SiteDirectory"."LogarithmicScale_Data"() AS "LogarithmicScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid")
  LEFT JOIN (SELECT "ScaleReferenceQuantityValue"."Container" AS "Iid", array_agg("ScaleReferenceQuantityValue"."Iid"::text) AS "ReferenceQuantityValue"
   FROM "SiteDirectory"."ScaleReferenceQuantityValue_Data"() AS "ScaleReferenceQuantityValue"
   JOIN "SiteDirectory"."LogarithmicScale_Data"() AS "LogarithmicScale" ON "ScaleReferenceQuantityValue"."Container" = "LogarithmicScale"."Iid"
   GROUP BY "ScaleReferenceQuantityValue"."Container") AS "LogarithmicScale_ReferenceQuantityValue" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."UnitPrefix_View";

CREATE OR REPLACE VIEW "SiteDirectory"."UnitPrefix_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "UnitPrefix"."ValueTypeDictionary" AS "ValueTypeSet",
	"UnitPrefix"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."UnitPrefix_Data"() AS "UnitPrefix" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."MeasurementUnit_View";

CREATE OR REPLACE VIEW "SiteDirectory"."MeasurementUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DerivedUnit_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DerivedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "DerivedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DerivedUnit_UnitFactor"."UnitFactor",'{}'::text[]) AS "UnitFactor",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit" USING ("Iid")
  JOIN "SiteDirectory"."DerivedUnit_Data"() AS "DerivedUnit" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "UnitFactor"."Container" AS "Iid", ARRAY[array_agg("UnitFactor"."Sequence"::text), array_agg("UnitFactor"."Iid"::text)] AS "UnitFactor"
   FROM "SiteDirectory"."UnitFactor_Data"() AS "UnitFactor"
   JOIN "SiteDirectory"."DerivedUnit_Data"() AS "DerivedUnit" ON "UnitFactor"."Container" = "DerivedUnit"."Iid"
   GROUP BY "UnitFactor"."Container") AS "DerivedUnit_UnitFactor" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ConversionBasedUnit_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ConversionBasedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit" USING ("Iid")
  JOIN "SiteDirectory"."ConversionBasedUnit_Data"() AS "ConversionBasedUnit" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."LinearConversionUnit_View";

CREATE OR REPLACE VIEW "SiteDirectory"."LinearConversionUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" || "LinearConversionUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit" USING ("Iid")
  JOIN "SiteDirectory"."ConversionBasedUnit_Data"() AS "ConversionBasedUnit" USING ("Iid")
  JOIN "SiteDirectory"."LinearConversionUnit_Data"() AS "LinearConversionUnit" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."PrefixedUnit_View";

CREATE OR REPLACE VIEW "SiteDirectory"."PrefixedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" || "PrefixedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	"PrefixedUnit"."Prefix",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit" USING ("Iid")
  JOIN "SiteDirectory"."ConversionBasedUnit_Data"() AS "ConversionBasedUnit" USING ("Iid")
  JOIN "SiteDirectory"."PrefixedUnit_Data"() AS "PrefixedUnit" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."SimpleUnit_View";

CREATE OR REPLACE VIEW "SiteDirectory"."SimpleUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "SimpleUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit" USING ("Iid")
  JOIN "SiteDirectory"."SimpleUnit_Data"() AS "SimpleUnit" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."FileType_View";

CREATE OR REPLACE VIEW "SiteDirectory"."FileType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "FileType"."ValueTypeDictionary" AS "ValueTypeSet",
	"FileType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("FileType_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."FileType_Data"() AS "FileType" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "FileType" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."FileType_Category_Data"() AS "FileType_Category"
   JOIN "SiteDirectory"."FileType_Data"() AS "FileType" ON "FileType" = "Iid"
   GROUP BY "FileType") AS "FileType_Category" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."Glossary_View";

CREATE OR REPLACE VIEW "SiteDirectory"."Glossary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Glossary"."ValueTypeDictionary" AS "ValueTypeSet",
	"Glossary"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Glossary_Term"."Term",'{}'::text[]) AS "Term",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Glossary_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Glossary_Data"() AS "Glossary" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Glossary" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."Glossary_Category_Data"() AS "Glossary_Category"
   JOIN "SiteDirectory"."Glossary_Data"() AS "Glossary" ON "Glossary" = "Iid"
   GROUP BY "Glossary") AS "Glossary_Category" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Term"."Container" AS "Iid", array_agg("Term"."Iid"::text) AS "Term"
   FROM "SiteDirectory"."Term_Data"() AS "Term"
   JOIN "SiteDirectory"."Glossary_Data"() AS "Glossary" ON "Term"."Container" = "Glossary"."Iid"
   GROUP BY "Term"."Container") AS "Glossary_Term" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."Term_View";

CREATE OR REPLACE VIEW "SiteDirectory"."Term_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Term"."ValueTypeDictionary" AS "ValueTypeSet",
	"Term"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Term_Data"() AS "Term" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ReferenceSource_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ReferenceSource_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceSource"."ValueTypeDictionary" AS "ValueTypeSet",
	"ReferenceSource"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceSource"."Publisher",
	"ReferenceSource"."PublishedIn",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ReferenceSource_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ReferenceSource_Data"() AS "ReferenceSource" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceSource" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."ReferenceSource_Category_Data"() AS "ReferenceSource_Category"
   JOIN "SiteDirectory"."ReferenceSource_Data"() AS "ReferenceSource" ON "ReferenceSource" = "Iid"
   GROUP BY "ReferenceSource") AS "ReferenceSource_Category" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."Rule_View";

CREATE OR REPLACE VIEW "SiteDirectory"."Rule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Rule_Data"() AS "Rule" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ReferencerRule_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ReferencerRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "ReferencerRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"ReferencerRule"."ReferencingCategory",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ReferencerRule_ReferencedCategory"."ReferencedCategory",'{}'::text[]) AS "ReferencedCategory"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Rule_Data"() AS "Rule" USING ("Iid")
  JOIN "SiteDirectory"."ReferencerRule_Data"() AS "ReferencerRule" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ReferencerRule" AS "Iid", array_agg("ReferencedCategory"::text) AS "ReferencedCategory"
   FROM "SiteDirectory"."ReferencerRule_ReferencedCategory_Data"() AS "ReferencerRule_ReferencedCategory"
   JOIN "SiteDirectory"."ReferencerRule_Data"() AS "ReferencerRule" ON "ReferencerRule" = "Iid"
   GROUP BY "ReferencerRule") AS "ReferencerRule_ReferencedCategory" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."BinaryRelationshipRule_View";

CREATE OR REPLACE VIEW "SiteDirectory"."BinaryRelationshipRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "BinaryRelationshipRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"BinaryRelationshipRule"."RelationshipCategory",
	"BinaryRelationshipRule"."SourceCategory",
	"BinaryRelationshipRule"."TargetCategory",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Rule_Data"() AS "Rule" USING ("Iid")
  JOIN "SiteDirectory"."BinaryRelationshipRule_Data"() AS "BinaryRelationshipRule" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."MultiRelationshipRule_View";

CREATE OR REPLACE VIEW "SiteDirectory"."MultiRelationshipRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "MultiRelationshipRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"MultiRelationshipRule"."RelationshipCategory",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("MultiRelationshipRule_RelatedCategory"."RelatedCategory",'{}'::text[]) AS "RelatedCategory"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Rule_Data"() AS "Rule" USING ("Iid")
  JOIN "SiteDirectory"."MultiRelationshipRule_Data"() AS "MultiRelationshipRule" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "MultiRelationshipRule" AS "Iid", array_agg("RelatedCategory"::text) AS "RelatedCategory"
   FROM "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Data"() AS "MultiRelationshipRule_RelatedCategory"
   JOIN "SiteDirectory"."MultiRelationshipRule_Data"() AS "MultiRelationshipRule" ON "MultiRelationshipRule" = "Iid"
   GROUP BY "MultiRelationshipRule") AS "MultiRelationshipRule_RelatedCategory" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DecompositionRule_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DecompositionRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "DecompositionRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"DecompositionRule"."ContainingCategory",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("DecompositionRule_ContainedCategory"."ContainedCategory",'{}'::text[]) AS "ContainedCategory"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Rule_Data"() AS "Rule" USING ("Iid")
  JOIN "SiteDirectory"."DecompositionRule_Data"() AS "DecompositionRule" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "DecompositionRule" AS "Iid", array_agg("ContainedCategory"::text) AS "ContainedCategory"
   FROM "SiteDirectory"."DecompositionRule_ContainedCategory_Data"() AS "DecompositionRule_ContainedCategory"
   JOIN "SiteDirectory"."DecompositionRule_Data"() AS "DecompositionRule" ON "DecompositionRule" = "Iid"
   GROUP BY "DecompositionRule") AS "DecompositionRule_ContainedCategory" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."ParameterizedCategoryRule_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ParameterizedCategoryRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "ParameterizedCategoryRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"ParameterizedCategoryRule"."Category",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ParameterizedCategoryRule_ParameterType"."ParameterType",'{}'::text[]) AS "ParameterType"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Rule_Data"() AS "Rule" USING ("Iid")
  JOIN "SiteDirectory"."ParameterizedCategoryRule_Data"() AS "ParameterizedCategoryRule" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ParameterizedCategoryRule" AS "Iid", array_agg("ParameterType"::text) AS "ParameterType"
   FROM "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Data"() AS "ParameterizedCategoryRule_ParameterType"
   JOIN "SiteDirectory"."ParameterizedCategoryRule_Data"() AS "ParameterizedCategoryRule" ON "ParameterizedCategoryRule" = "Iid"
   GROUP BY "ParameterizedCategoryRule") AS "ParameterizedCategoryRule_ParameterType" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."Constant_View";

CREATE OR REPLACE VIEW "SiteDirectory"."Constant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Constant"."ValueTypeDictionary" AS "ValueTypeSet",
	"Constant"."Container",
	NULL::bigint AS "Sequence",
	"Constant"."ParameterType",
	"Constant"."Scale",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Constant_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."Constant_Data"() AS "Constant" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Constant" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."Constant_Category_Data"() AS "Constant_Category"
   JOIN "SiteDirectory"."Constant_Data"() AS "Constant" ON "Constant" = "Iid"
   GROUP BY "Constant") AS "Constant_Category" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."EngineeringModelSetup_View";

CREATE OR REPLACE VIEW "SiteDirectory"."EngineeringModelSetup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EngineeringModelSetup"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelSetup"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelSetup"."DefaultOrganizationalParticipant",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
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

DROP VIEW IF EXISTS "SiteDirectory"."ModelReferenceDataLibrary_View";

CREATE OR REPLACE VIEW "SiteDirectory"."ModelReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" || "ModelReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModelReferenceDataLibrary"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceDataLibrary"."RequiredRdl",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("ReferenceDataLibrary_DefinedCategory"."DefinedCategory",'{}'::text[]) AS "DefinedCategory",
	COALESCE("ReferenceDataLibrary_ParameterType"."ParameterType",'{}'::text[]) AS "ParameterType",
	COALESCE("ReferenceDataLibrary_Scale"."Scale",'{}'::text[]) AS "Scale",
	COALESCE("ReferenceDataLibrary_UnitPrefix"."UnitPrefix",'{}'::text[]) AS "UnitPrefix",
	COALESCE("ReferenceDataLibrary_Unit"."Unit",'{}'::text[]) AS "Unit",
	COALESCE("ReferenceDataLibrary_FileType"."FileType",'{}'::text[]) AS "FileType",
	COALESCE("ReferenceDataLibrary_Glossary"."Glossary",'{}'::text[]) AS "Glossary",
	COALESCE("ReferenceDataLibrary_ReferenceSource"."ReferenceSource",'{}'::text[]) AS "ReferenceSource",
	COALESCE("ReferenceDataLibrary_Rule"."Rule",'{}'::text[]) AS "Rule",
	COALESCE("ReferenceDataLibrary_Constant"."Constant",'{}'::text[]) AS "Constant",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("ReferenceDataLibrary_BaseQuantityKind"."BaseQuantityKind",'{}'::text[]) AS "BaseQuantityKind",
	COALESCE("ReferenceDataLibrary_BaseUnit"."BaseUnit",'{}'::text[]) AS "BaseUnit"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" USING ("Iid")
  JOIN "SiteDirectory"."ModelReferenceDataLibrary_Data"() AS "ModelReferenceDataLibrary" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceDataLibrary" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("BaseQuantityKind"::text)] AS "BaseQuantityKind"
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Data"() AS "ReferenceDataLibrary_BaseQuantityKind"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceDataLibrary" = "Iid"
   GROUP BY "ReferenceDataLibrary") AS "ReferenceDataLibrary_BaseQuantityKind" USING ("Iid")
 LEFT JOIN (SELECT "ReferenceDataLibrary" AS "Iid", array_agg("BaseUnit"::text) AS "BaseUnit"
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Data"() AS "ReferenceDataLibrary_BaseUnit"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceDataLibrary" = "Iid"
   GROUP BY "ReferenceDataLibrary") AS "ReferenceDataLibrary_BaseUnit" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Category"."Container" AS "Iid", array_agg("Category"."Iid"::text) AS "DefinedCategory"
   FROM "SiteDirectory"."Category_Data"() AS "Category"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Category"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Category"."Container") AS "ReferenceDataLibrary_DefinedCategory" USING ("Iid")
  LEFT JOIN (SELECT "ParameterType"."Container" AS "Iid", array_agg("ParameterType"."Iid"::text) AS "ParameterType"
   FROM "SiteDirectory"."ParameterType_Data"() AS "ParameterType"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ParameterType"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "ParameterType"."Container") AS "ReferenceDataLibrary_ParameterType" USING ("Iid")
  LEFT JOIN (SELECT "MeasurementScale"."Container" AS "Iid", array_agg("MeasurementScale"."Iid"::text) AS "Scale"
   FROM "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "MeasurementScale"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "MeasurementScale"."Container") AS "ReferenceDataLibrary_Scale" USING ("Iid")
  LEFT JOIN (SELECT "UnitPrefix"."Container" AS "Iid", array_agg("UnitPrefix"."Iid"::text) AS "UnitPrefix"
   FROM "SiteDirectory"."UnitPrefix_Data"() AS "UnitPrefix"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "UnitPrefix"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "UnitPrefix"."Container") AS "ReferenceDataLibrary_UnitPrefix" USING ("Iid")
  LEFT JOIN (SELECT "MeasurementUnit"."Container" AS "Iid", array_agg("MeasurementUnit"."Iid"::text) AS "Unit"
   FROM "SiteDirectory"."MeasurementUnit_Data"() AS "MeasurementUnit"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "MeasurementUnit"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "MeasurementUnit"."Container") AS "ReferenceDataLibrary_Unit" USING ("Iid")
  LEFT JOIN (SELECT "FileType"."Container" AS "Iid", array_agg("FileType"."Iid"::text) AS "FileType"
   FROM "SiteDirectory"."FileType_Data"() AS "FileType"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "FileType"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "FileType"."Container") AS "ReferenceDataLibrary_FileType" USING ("Iid")
  LEFT JOIN (SELECT "Glossary"."Container" AS "Iid", array_agg("Glossary"."Iid"::text) AS "Glossary"
   FROM "SiteDirectory"."Glossary_Data"() AS "Glossary"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Glossary"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Glossary"."Container") AS "ReferenceDataLibrary_Glossary" USING ("Iid")
  LEFT JOIN (SELECT "ReferenceSource"."Container" AS "Iid", array_agg("ReferenceSource"."Iid"::text) AS "ReferenceSource"
   FROM "SiteDirectory"."ReferenceSource_Data"() AS "ReferenceSource"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "ReferenceSource"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "ReferenceSource"."Container") AS "ReferenceDataLibrary_ReferenceSource" USING ("Iid")
  LEFT JOIN (SELECT "Rule"."Container" AS "Iid", array_agg("Rule"."Iid"::text) AS "Rule"
   FROM "SiteDirectory"."Rule_Data"() AS "Rule"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Rule"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Rule"."Container") AS "ReferenceDataLibrary_Rule" USING ("Iid")
  LEFT JOIN (SELECT "Constant"."Container" AS "Iid", array_agg("Constant"."Iid"::text) AS "Constant"
   FROM "SiteDirectory"."Constant_Data"() AS "Constant"
   JOIN "SiteDirectory"."ReferenceDataLibrary_Data"() AS "ReferenceDataLibrary" ON "Constant"."Container" = "ReferenceDataLibrary"."Iid"
   GROUP BY "Constant"."Container") AS "ReferenceDataLibrary_Constant" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."PersonRole_View";

CREATE OR REPLACE VIEW "SiteDirectory"."PersonRole_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PersonRole"."ValueTypeDictionary" AS "ValueTypeSet",
	"PersonRole"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("PersonRole_PersonPermission"."PersonPermission",'{}'::text[]) AS "PersonPermission",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."PersonRole_Data"() AS "PersonRole" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "PersonPermission"."Container" AS "Iid", array_agg("PersonPermission"."Iid"::text) AS "PersonPermission"
   FROM "SiteDirectory"."PersonPermission_Data"() AS "PersonPermission"
   JOIN "SiteDirectory"."PersonRole_Data"() AS "PersonRole" ON "PersonPermission"."Container" = "PersonRole"."Iid"
   GROUP BY "PersonPermission"."Container") AS "PersonRole_PersonPermission" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DomainOfExpertiseGroup_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DomainOfExpertiseGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "DomainOfExpertiseGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainOfExpertiseGroup"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("DomainOfExpertiseGroup_Domain"."Domain",'{}'::text[]) AS "Domain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."DomainOfExpertiseGroup_Data"() AS "DomainOfExpertiseGroup" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "DomainOfExpertiseGroup" AS "Iid", array_agg("Domain"::text) AS "Domain"
   FROM "SiteDirectory"."DomainOfExpertiseGroup_Domain_Data"() AS "DomainOfExpertiseGroup_Domain"
   JOIN "SiteDirectory"."DomainOfExpertiseGroup_Data"() AS "DomainOfExpertiseGroup" ON "DomainOfExpertiseGroup" = "Iid"
   GROUP BY "DomainOfExpertiseGroup") AS "DomainOfExpertiseGroup_Domain" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");

DROP VIEW IF EXISTS "SiteDirectory"."DomainOfExpertise_View";

CREATE OR REPLACE VIEW "SiteDirectory"."DomainOfExpertise_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "DomainOfExpertise"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainOfExpertise"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("DefinedThing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("DomainOfExpertise_Category"."Category",'{}'::text[]) AS "Category"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" USING ("Iid")
  JOIN "SiteDirectory"."DomainOfExpertise_Data"() AS "DomainOfExpertise" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "DomainOfExpertise" AS "Iid", array_agg("Category"::text) AS "Category"
   FROM "SiteDirectory"."DomainOfExpertise_Category_Data"() AS "DomainOfExpertise_Category"
   JOIN "SiteDirectory"."DomainOfExpertise_Data"() AS "DomainOfExpertise" ON "DomainOfExpertise" = "Iid"
   GROUP BY "DomainOfExpertise") AS "DomainOfExpertise_Category" USING ("Iid")
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."DefinedThing_Data"() AS "DefinedThing" ON "Attachment"."Container" = "DefinedThing"."Iid"
   GROUP BY "Attachment"."Container") AS "DefinedThing_Attachment" USING ("Iid");