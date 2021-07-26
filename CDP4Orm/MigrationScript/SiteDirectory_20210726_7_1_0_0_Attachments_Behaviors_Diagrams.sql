-- Create table for class Attachment
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

-- Attachment is contained (composite) by Thing: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Attachment" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Attachment" ADD CONSTRAINT "Attachment_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Attachment_Container" ON "SiteDirectory"."Attachment" ("Container");
CREATE TRIGGER attachment_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Attachment"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');

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

CREATE OR REPLACE VIEW "SiteDirectory"."Thing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."TopContainer_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TopContainer"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."TopContainer_Data"() AS "TopContainer" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SiteDirectory_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TopContainer"."ValueTypeDictionary" || "SiteDirectory"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectory"."DefaultParticipantRole",
	"SiteDirectory"."DefaultPersonRole",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("SiteDirectory_Organization"."Organization",'{}'::text[]) AS "Organization",
	COALESCE("SiteDirectory_Person"."Person",'{}'::text[]) AS "Person",
	COALESCE("SiteDirectory_ParticipantRole"."ParticipantRole",'{}'::text[]) AS "ParticipantRole",
	COALESCE("SiteDirectory_SiteReferenceDataLibrary"."SiteReferenceDataLibrary",'{}'::text[]) AS "SiteReferenceDataLibrary",
	COALESCE("SiteDirectory_Model"."Model",'{}'::text[]) AS "Model",
	COALESCE("SiteDirectory_PersonRole"."PersonRole",'{}'::text[]) AS "PersonRole",
	COALESCE("SiteDirectory_LogEntry"."LogEntry",'{}'::text[]) AS "LogEntry",
	COALESCE("SiteDirectory_DomainGroup"."DomainGroup",'{}'::text[]) AS "DomainGroup",
	COALESCE("SiteDirectory_Domain"."Domain",'{}'::text[]) AS "Domain",
	COALESCE("SiteDirectory_NaturalLanguage"."NaturalLanguage",'{}'::text[]) AS "NaturalLanguage",
	COALESCE("SiteDirectory_Annotation"."Annotation",'{}'::text[]) AS "Annotation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."TopContainer_Data"() AS "TopContainer" USING ("Iid")
  JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Organization"."Container" AS "Iid", array_agg("Organization"."Iid"::text) AS "Organization"
   FROM "SiteDirectory"."Organization_Data"() AS "Organization"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "Organization"."Container" = "SiteDirectory"."Iid"
   GROUP BY "Organization"."Container") AS "SiteDirectory_Organization" USING ("Iid")
  LEFT JOIN (SELECT "Person"."Container" AS "Iid", array_agg("Person"."Iid"::text) AS "Person"
   FROM "SiteDirectory"."Person_Data"() AS "Person"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "Person"."Container" = "SiteDirectory"."Iid"
   GROUP BY "Person"."Container") AS "SiteDirectory_Person" USING ("Iid")
  LEFT JOIN (SELECT "ParticipantRole"."Container" AS "Iid", array_agg("ParticipantRole"."Iid"::text) AS "ParticipantRole"
   FROM "SiteDirectory"."ParticipantRole_Data"() AS "ParticipantRole"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "ParticipantRole"."Container" = "SiteDirectory"."Iid"
   GROUP BY "ParticipantRole"."Container") AS "SiteDirectory_ParticipantRole" USING ("Iid")
  LEFT JOIN (SELECT "SiteReferenceDataLibrary"."Container" AS "Iid", array_agg("SiteReferenceDataLibrary"."Iid"::text) AS "SiteReferenceDataLibrary"
   FROM "SiteDirectory"."SiteReferenceDataLibrary_Data"() AS "SiteReferenceDataLibrary"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "SiteReferenceDataLibrary"."Container" = "SiteDirectory"."Iid"
   GROUP BY "SiteReferenceDataLibrary"."Container") AS "SiteDirectory_SiteReferenceDataLibrary" USING ("Iid")
  LEFT JOIN (SELECT "EngineeringModelSetup"."Container" AS "Iid", array_agg("EngineeringModelSetup"."Iid"::text) AS "Model"
   FROM "SiteDirectory"."EngineeringModelSetup_Data"() AS "EngineeringModelSetup"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "EngineeringModelSetup"."Container" = "SiteDirectory"."Iid"
   GROUP BY "EngineeringModelSetup"."Container") AS "SiteDirectory_Model" USING ("Iid")
  LEFT JOIN (SELECT "PersonRole"."Container" AS "Iid", array_agg("PersonRole"."Iid"::text) AS "PersonRole"
   FROM "SiteDirectory"."PersonRole_Data"() AS "PersonRole"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "PersonRole"."Container" = "SiteDirectory"."Iid"
   GROUP BY "PersonRole"."Container") AS "SiteDirectory_PersonRole" USING ("Iid")
  LEFT JOIN (SELECT "SiteLogEntry"."Container" AS "Iid", array_agg("SiteLogEntry"."Iid"::text) AS "LogEntry"
   FROM "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "SiteLogEntry"."Container" = "SiteDirectory"."Iid"
   GROUP BY "SiteLogEntry"."Container") AS "SiteDirectory_LogEntry" USING ("Iid")
  LEFT JOIN (SELECT "DomainOfExpertiseGroup"."Container" AS "Iid", array_agg("DomainOfExpertiseGroup"."Iid"::text) AS "DomainGroup"
   FROM "SiteDirectory"."DomainOfExpertiseGroup_Data"() AS "DomainOfExpertiseGroup"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "DomainOfExpertiseGroup"."Container" = "SiteDirectory"."Iid"
   GROUP BY "DomainOfExpertiseGroup"."Container") AS "SiteDirectory_DomainGroup" USING ("Iid")
  LEFT JOIN (SELECT "DomainOfExpertise"."Container" AS "Iid", array_agg("DomainOfExpertise"."Iid"::text) AS "Domain"
   FROM "SiteDirectory"."DomainOfExpertise_Data"() AS "DomainOfExpertise"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "DomainOfExpertise"."Container" = "SiteDirectory"."Iid"
   GROUP BY "DomainOfExpertise"."Container") AS "SiteDirectory_Domain" USING ("Iid")
  LEFT JOIN (SELECT "NaturalLanguage"."Container" AS "Iid", array_agg("NaturalLanguage"."Iid"::text) AS "NaturalLanguage"
   FROM "SiteDirectory"."NaturalLanguage_Data"() AS "NaturalLanguage"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "NaturalLanguage"."Container" = "SiteDirectory"."Iid"
   GROUP BY "NaturalLanguage"."Container") AS "SiteDirectory_NaturalLanguage" USING ("Iid")
  LEFT JOIN (SELECT "SiteDirectoryDataAnnotation"."Container" AS "Iid", array_agg("SiteDirectoryDataAnnotation"."Iid"::text) AS "Annotation"
   FROM "SiteDirectory"."SiteDirectoryDataAnnotation_Data"() AS "SiteDirectoryDataAnnotation"
   JOIN "SiteDirectory"."SiteDirectory_Data"() AS "SiteDirectory" ON "SiteDirectoryDataAnnotation"."Container" = "SiteDirectory"."Iid"
   GROUP BY "SiteDirectoryDataAnnotation"."Container") AS "SiteDirectory_Annotation" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Attachment_View" AS
 SELECT "Thing"."Iid", "Attachment"."ValueTypeDictionary" AS "ValueTypeSet",
	"Attachment"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Attachment_FileType"."FileType",'{}'::text[]) AS "FileType"
  FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
  LEFT JOIN (SELECT "Attachment" AS "Iid", array_agg("FileType"::text) AS "FileType"
   FROM "SiteDirectory"."Attachment_FileType_Data"() AS "Attachment_FileType"
   JOIN "SiteDirectory"."Attachment_Data"() AS "Attachment" ON "Attachment" = "Iid"
   GROUP BY "Attachment") AS "Attachment_FileType" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Organization_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Organization"."ValueTypeDictionary" AS "ValueTypeSet",
	"Organization"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Organization_Data"() AS "Organization" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Person_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Person"."ValueTypeDictionary" AS "ValueTypeSet",
	"Person"."Container",
	NULL::bigint AS "Sequence",
	"Person"."Organization",
	"Person"."DefaultDomain",
	"Person"."Role",
	"Person"."DefaultEmailAddress",
	"Person"."DefaultTelephoneNumber",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Person_EmailAddress"."EmailAddress",'{}'::text[]) AS "EmailAddress",
	COALESCE("Person_TelephoneNumber"."TelephoneNumber",'{}'::text[]) AS "TelephoneNumber",
	COALESCE("Person_UserPreference"."UserPreference",'{}'::text[]) AS "UserPreference",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Person_Data"() AS "Person" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "EmailAddress"."Container" AS "Iid", array_agg("EmailAddress"."Iid"::text) AS "EmailAddress"
   FROM "SiteDirectory"."EmailAddress_Data"() AS "EmailAddress"
   JOIN "SiteDirectory"."Person_Data"() AS "Person" ON "EmailAddress"."Container" = "Person"."Iid"
   GROUP BY "EmailAddress"."Container") AS "Person_EmailAddress" USING ("Iid")
  LEFT JOIN (SELECT "TelephoneNumber"."Container" AS "Iid", array_agg("TelephoneNumber"."Iid"::text) AS "TelephoneNumber"
   FROM "SiteDirectory"."TelephoneNumber_Data"() AS "TelephoneNumber"
   JOIN "SiteDirectory"."Person_Data"() AS "Person" ON "TelephoneNumber"."Container" = "Person"."Iid"
   GROUP BY "TelephoneNumber"."Container") AS "Person_TelephoneNumber" USING ("Iid")
  LEFT JOIN (SELECT "UserPreference"."Container" AS "Iid", array_agg("UserPreference"."Iid"::text) AS "UserPreference"
   FROM "SiteDirectory"."UserPreference_Data"() AS "UserPreference"
   JOIN "SiteDirectory"."Person_Data"() AS "Person" ON "UserPreference"."Container" = "Person"."Iid"
   GROUP BY "UserPreference"."Container") AS "Person_UserPreference" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."EmailAddress_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "EmailAddress"."ValueTypeDictionary" AS "ValueTypeSet",
	"EmailAddress"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."EmailAddress_Data"() AS "EmailAddress" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."TelephoneNumber_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TelephoneNumber"."ValueTypeDictionary" AS "ValueTypeSet",
	"TelephoneNumber"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("TelephoneNumber_VcardType"."VcardType",'{}'::text[]) AS "VcardType"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."TelephoneNumber_Data"() AS "TelephoneNumber" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "TelephoneNumber" AS "Iid", array_agg("VcardType"::text) AS "VcardType"
   FROM "SiteDirectory"."TelephoneNumber_VcardType_Data"() AS "TelephoneNumber_VcardType"
   JOIN "SiteDirectory"."TelephoneNumber_Data"() AS "TelephoneNumber" ON "TelephoneNumber" = "Iid"
   GROUP BY "TelephoneNumber") AS "TelephoneNumber_VcardType" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."UserPreference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "UserPreference"."ValueTypeDictionary" AS "ValueTypeSet",
	"UserPreference"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."UserPreference_Data"() AS "UserPreference" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DefinedThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ParticipantRole_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParticipantRole"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParticipantRole"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ParticipantPermission"."Container" AS "Iid", array_agg("ParticipantPermission"."Iid"::text) AS "ParticipantPermission"
   FROM "SiteDirectory"."ParticipantPermission_Data"() AS "ParticipantPermission"
   JOIN "SiteDirectory"."ParticipantRole_Data"() AS "ParticipantRole" ON "ParticipantPermission"."Container" = "ParticipantRole"."Iid"
   GROUP BY "ParticipantPermission"."Container") AS "ParticipantRole_ParticipantPermission" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Alias_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Alias"."ValueTypeDictionary" AS "ValueTypeSet",
	"Alias"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Alias_Data"() AS "Alias" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Definition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Definition"."ValueTypeDictionary" AS "ValueTypeSet",
	"Definition"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Definition_Citation"."Citation",'{}'::text[]) AS "Citation",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Definition_Note"."Note",'{}'::text[]) AS "Note",
	COALESCE("Definition_Example"."Example",'{}'::text[]) AS "Example"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Definition_Data"() AS "Definition" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Definition" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("Note"::text)] AS "Note"
   FROM "SiteDirectory"."Definition_Note_Data"() AS "Definition_Note"
   JOIN "SiteDirectory"."Definition_Data"() AS "Definition" ON "Definition" = "Iid"
   GROUP BY "Definition") AS "Definition_Note" USING ("Iid")
 LEFT JOIN (SELECT "Definition" AS "Iid", ARRAY[array_agg("Sequence"::text), array_agg("Example"::text)] AS "Example"
   FROM "SiteDirectory"."Definition_Example_Data"() AS "Definition_Example"
   JOIN "SiteDirectory"."Definition_Data"() AS "Definition" ON "Definition" = "Iid"
   GROUP BY "Definition") AS "Definition_Example" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "Citation"."Container" AS "Iid", array_agg("Citation"."Iid"::text) AS "Citation"
   FROM "SiteDirectory"."Citation_Data"() AS "Citation"
   JOIN "SiteDirectory"."Definition_Data"() AS "Definition" ON "Citation"."Container" = "Definition"."Iid"
   GROUP BY "Citation"."Container") AS "Definition_Citation" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Citation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Citation"."ValueTypeDictionary" AS "ValueTypeSet",
	"Citation"."Container",
	NULL::bigint AS "Sequence",
	"Citation"."Source",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Citation_Data"() AS "Citation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."HyperLink_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "HyperLink"."ValueTypeDictionary" AS "ValueTypeSet",
	"HyperLink"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."HyperLink_Data"() AS "HyperLink" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ParticipantPermission_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParticipantPermission"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParticipantPermission"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."ParticipantPermission_Data"() AS "ParticipantPermission" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"ReferenceDataLibrary"."RequiredRdl",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SiteDirectory"."SiteReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" || "SiteReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteReferenceDataLibrary"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceDataLibrary"."RequiredRdl",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SiteDirectory"."Category_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Category"."ValueTypeDictionary" AS "ValueTypeSet",
	"Category"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."CompoundParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "CompoundParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ParameterTypeComponent"."Container" AS "Iid", ARRAY[array_agg("ParameterTypeComponent"."Sequence"::text), array_agg("ParameterTypeComponent"."Iid"::text)] AS "Component"
   FROM "SiteDirectory"."ParameterTypeComponent_Data"() AS "ParameterTypeComponent"
   JOIN "SiteDirectory"."CompoundParameterType_Data"() AS "CompoundParameterType" ON "ParameterTypeComponent"."Container" = "CompoundParameterType"."Iid"
   GROUP BY "ParameterTypeComponent"."Container") AS "CompoundParameterType_Component" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ArrayParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "CompoundParameterType"."ValueTypeDictionary" || "ArrayParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ParameterTypeComponent"."Container" AS "Iid", ARRAY[array_agg("ParameterTypeComponent"."Sequence"::text), array_agg("ParameterTypeComponent"."Iid"::text)] AS "Component"
   FROM "SiteDirectory"."ParameterTypeComponent_Data"() AS "ParameterTypeComponent"
   JOIN "SiteDirectory"."CompoundParameterType_Data"() AS "CompoundParameterType" ON "ParameterTypeComponent"."Container" = "CompoundParameterType"."Iid"
   GROUP BY "ParameterTypeComponent"."Container") AS "CompoundParameterType_Component" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ParameterTypeComponent_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterTypeComponent"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterTypeComponent"."Container",
	"ParameterTypeComponent"."Sequence",
	"ParameterTypeComponent"."ParameterType",
	"ParameterTypeComponent"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."ParameterTypeComponent_Data"() AS "ParameterTypeComponent" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ScalarParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."EnumerationParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "EnumerationParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "EnumerationValueDefinition"."Container" AS "Iid", ARRAY[array_agg("EnumerationValueDefinition"."Sequence"::text), array_agg("EnumerationValueDefinition"."Iid"::text)] AS "ValueDefinition"
   FROM "SiteDirectory"."EnumerationValueDefinition_Data"() AS "EnumerationValueDefinition"
   JOIN "SiteDirectory"."EnumerationParameterType_Data"() AS "EnumerationParameterType" ON "EnumerationValueDefinition"."Container" = "EnumerationParameterType"."Iid"
   GROUP BY "EnumerationValueDefinition"."Container") AS "EnumerationParameterType_ValueDefinition" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."EnumerationValueDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EnumerationValueDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"EnumerationValueDefinition"."Container",
	"EnumerationValueDefinition"."Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."BooleanParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "BooleanParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DateParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "DateParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."TextParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "TextParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DateTimeParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "DateTimeParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."TimeOfDayParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "TimeOfDayParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."QuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SpecializedQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "SpecializedQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	"SpecializedQuantityKind"."General",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SimpleQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "SimpleQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DerivedQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "DerivedQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "QuantityKindFactor"."Container" AS "Iid", ARRAY[array_agg("QuantityKindFactor"."Sequence"::text), array_agg("QuantityKindFactor"."Iid"::text)] AS "QuantityKindFactor"
   FROM "SiteDirectory"."QuantityKindFactor_Data"() AS "QuantityKindFactor"
   JOIN "SiteDirectory"."DerivedQuantityKind_Data"() AS "DerivedQuantityKind" ON "QuantityKindFactor"."Container" = "DerivedQuantityKind"."Iid"
   GROUP BY "QuantityKindFactor"."Container") AS "DerivedQuantityKind_QuantityKindFactor" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."QuantityKindFactor_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "QuantityKindFactor"."ValueTypeDictionary" AS "ValueTypeSet",
	"QuantityKindFactor"."Container",
	"QuantityKindFactor"."Sequence",
	"QuantityKindFactor"."QuantityKind",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."QuantityKindFactor_Data"() AS "QuantityKindFactor" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SampledFunctionParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "SampledFunctionParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SiteDirectory"."IndependentParameterTypeAssignment_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "IndependentParameterTypeAssignment"."ValueTypeDictionary" AS "ValueTypeSet",
	"IndependentParameterTypeAssignment"."Container",
	"IndependentParameterTypeAssignment"."Sequence",
	"IndependentParameterTypeAssignment"."MeasurementScale",
	"IndependentParameterTypeAssignment"."ParameterType",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DependentParameterTypeAssignment_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DependentParameterTypeAssignment"."ValueTypeDictionary" AS "ValueTypeSet",
	"DependentParameterTypeAssignment"."Container",
	"DependentParameterTypeAssignment"."Sequence",
	"DependentParameterTypeAssignment"."MeasurementScale",
	"DependentParameterTypeAssignment"."ParameterType",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."MeasurementScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."OrdinalScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "OrdinalScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ScaleValueDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ScaleValueDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"ScaleValueDefinition"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."MappingToReferenceScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "MappingToReferenceScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MappingToReferenceScale"."Container",
	NULL::bigint AS "Sequence",
	"MappingToReferenceScale"."ReferenceScaleValue",
	"MappingToReferenceScale"."DependentScaleValue",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."RatioScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "RatioScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."CyclicRatioScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "RatioScale"."ValueTypeDictionary" || "CyclicRatioScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."IntervalScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "IntervalScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "ScaleValueDefinition"."Container" AS "Iid", array_agg("ScaleValueDefinition"."Iid"::text) AS "ValueDefinition"
   FROM "SiteDirectory"."ScaleValueDefinition_Data"() AS "ScaleValueDefinition"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "ScaleValueDefinition"."Container" = "MeasurementScale"."Iid"
   GROUP BY "ScaleValueDefinition"."Container") AS "MeasurementScale_ValueDefinition" USING ("Iid")
  LEFT JOIN (SELECT "MappingToReferenceScale"."Container" AS "Iid", array_agg("MappingToReferenceScale"."Iid"::text) AS "MappingToReferenceScale"
   FROM "SiteDirectory"."MappingToReferenceScale_Data"() AS "MappingToReferenceScale"
   JOIN "SiteDirectory"."MeasurementScale_Data"() AS "MeasurementScale" ON "MappingToReferenceScale"."Container" = "MeasurementScale"."Iid"
   GROUP BY "MappingToReferenceScale"."Container") AS "MeasurementScale_MappingToReferenceScale" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."LogarithmicScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "LogarithmicScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	"LogarithmicScale"."ReferenceQuantityKind",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SiteDirectory"."ScaleReferenceQuantityValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ScaleReferenceQuantityValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"ScaleReferenceQuantityValue"."Container",
	NULL::bigint AS "Sequence",
	"ScaleReferenceQuantityValue"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."ScaleReferenceQuantityValue_Data"() AS "ScaleReferenceQuantityValue" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."UnitPrefix_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "UnitPrefix"."ValueTypeDictionary" AS "ValueTypeSet",
	"UnitPrefix"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."MeasurementUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DerivedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "DerivedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "UnitFactor"."Container" AS "Iid", ARRAY[array_agg("UnitFactor"."Sequence"::text), array_agg("UnitFactor"."Iid"::text)] AS "UnitFactor"
   FROM "SiteDirectory"."UnitFactor_Data"() AS "UnitFactor"
   JOIN "SiteDirectory"."DerivedUnit_Data"() AS "DerivedUnit" ON "UnitFactor"."Container" = "DerivedUnit"."Iid"
   GROUP BY "UnitFactor"."Container") AS "DerivedUnit_UnitFactor" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."UnitFactor_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "UnitFactor"."ValueTypeDictionary" AS "ValueTypeSet",
	"UnitFactor"."Container",
	"UnitFactor"."Sequence",
	"UnitFactor"."Unit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."UnitFactor_Data"() AS "UnitFactor" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ConversionBasedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."LinearConversionUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" || "LinearConversionUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."PrefixedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" || "PrefixedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	"PrefixedUnit"."Prefix",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SimpleUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "SimpleUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."FileType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "FileType"."ValueTypeDictionary" AS "ValueTypeSet",
	"FileType"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Glossary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Glossary"."ValueTypeDictionary" AS "ValueTypeSet",
	"Glossary"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "Term"."Container" AS "Iid", array_agg("Term"."Iid"::text) AS "Term"
   FROM "SiteDirectory"."Term_Data"() AS "Term"
   JOIN "SiteDirectory"."Glossary_Data"() AS "Glossary" ON "Term"."Container" = "Glossary"."Iid"
   GROUP BY "Term"."Container") AS "Glossary_Term" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Term_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Term"."ValueTypeDictionary" AS "ValueTypeSet",
	"Term"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ReferenceSource_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceSource"."ValueTypeDictionary" AS "ValueTypeSet",
	"ReferenceSource"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceSource"."Publisher",
	"ReferenceSource"."PublishedIn",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Rule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ReferencerRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "ReferencerRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"ReferencerRule"."ReferencingCategory",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."BinaryRelationshipRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "BinaryRelationshipRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"BinaryRelationshipRule"."RelationshipCategory",
	"BinaryRelationshipRule"."SourceCategory",
	"BinaryRelationshipRule"."TargetCategory",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."MultiRelationshipRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "MultiRelationshipRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"MultiRelationshipRule"."RelationshipCategory",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DecompositionRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "DecompositionRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"DecompositionRule"."ContainingCategory",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ParameterizedCategoryRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "ParameterizedCategoryRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"ParameterizedCategoryRule"."Category",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."Constant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Constant"."ValueTypeDictionary" AS "ValueTypeSet",
	"Constant"."Container",
	NULL::bigint AS "Sequence",
	"Constant"."ParameterType",
	"Constant"."Scale",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."EngineeringModelSetup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EngineeringModelSetup"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelSetup"."Container",
	NULL::bigint AS "Sequence",
	"EngineeringModelSetup"."DefaultOrganizationalParticipant",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SiteDirectory"."Participant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Participant"."ValueTypeDictionary" AS "ValueTypeSet",
	"Participant"."Container",
	NULL::bigint AS "Sequence",
	"Participant"."Person",
	"Participant"."Role",
	"Participant"."SelectedDomain",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("Participant_Domain"."Domain",'{}'::text[]) AS "Domain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."Participant_Data"() AS "Participant" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
 LEFT JOIN (SELECT "Participant" AS "Iid", array_agg("Domain"::text) AS "Domain"
   FROM "SiteDirectory"."Participant_Domain_Data"() AS "Participant_Domain"
   JOIN "SiteDirectory"."Participant_Data"() AS "Participant" ON "Participant" = "Iid"
   GROUP BY "Participant") AS "Participant_Domain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ModelReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" || "ModelReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModelReferenceDataLibrary"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceDataLibrary"."RequiredRdl",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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

CREATE OR REPLACE VIEW "SiteDirectory"."IterationSetup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "IterationSetup"."ValueTypeDictionary" AS "ValueTypeSet",
	"IterationSetup"."Container",
	NULL::bigint AS "Sequence",
	"IterationSetup"."SourceIterationSetup",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."IterationSetup_Data"() AS "IterationSetup" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."OrganizationalParticipant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "OrganizationalParticipant"."ValueTypeDictionary" AS "ValueTypeSet",
	"OrganizationalParticipant"."Container",
	NULL::bigint AS "Sequence",
	"OrganizationalParticipant"."Organization",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."PersonRole_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PersonRole"."ValueTypeDictionary" AS "ValueTypeSet",
	"PersonRole"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
  LEFT JOIN (SELECT "PersonPermission"."Container" AS "Iid", array_agg("PersonPermission"."Iid"::text) AS "PersonPermission"
   FROM "SiteDirectory"."PersonPermission_Data"() AS "PersonPermission"
   JOIN "SiteDirectory"."PersonRole_Data"() AS "PersonRole" ON "PersonPermission"."Container" = "PersonRole"."Iid"
   GROUP BY "PersonPermission"."Container") AS "PersonRole_PersonPermission" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."PersonPermission_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "PersonPermission"."ValueTypeDictionary" AS "ValueTypeSet",
	"PersonPermission"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."PersonPermission_Data"() AS "PersonPermission" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SiteLogEntry_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "SiteLogEntry"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteLogEntry"."Container",
	NULL::bigint AS "Sequence",
	"SiteLogEntry"."Author",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "LogEntryChangelogItem"."Container" AS "Iid", array_agg("LogEntryChangelogItem"."Iid"::text) AS "LogEntryChangelogItem"
   FROM "SiteDirectory"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem"
   JOIN "SiteDirectory"."SiteLogEntry_Data"() AS "SiteLogEntry" ON "LogEntryChangelogItem"."Container" = "SiteLogEntry"."Iid"
   GROUP BY "LogEntryChangelogItem"."Container") AS "SiteLogEntry_LogEntryChangelogItem" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."LogEntryChangelogItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "LogEntryChangelogItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"LogEntryChangelogItem"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
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
   GROUP BY "LogEntryChangelogItem") AS "LogEntryChangelogItem_AffectedReferenceIid" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DomainOfExpertiseGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "DomainOfExpertiseGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainOfExpertiseGroup"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DomainOfExpertise_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "DomainOfExpertise"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainOfExpertise"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
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
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
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
   GROUP BY "HyperLink"."Container") AS "DefinedThing_HyperLink" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."NaturalLanguage_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "NaturalLanguage"."ValueTypeDictionary" AS "ValueTypeSet",
	"NaturalLanguage"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."NaturalLanguage_Data"() AS "NaturalLanguage" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."GenericAnnotation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" AS "ValueTypeSet",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SiteDirectoryDataAnnotation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "SiteDirectoryDataAnnotation"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectoryDataAnnotation"."Container",
	NULL::bigint AS "Sequence",
	"SiteDirectoryDataAnnotation"."Author",
	"SiteDirectoryDataAnnotation"."PrimaryAnnotatedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("SiteDirectoryDataAnnotation_RelatedThing"."RelatedThing",'{}'::text[]) AS "RelatedThing",
	COALESCE("SiteDirectoryDataAnnotation_Discussion"."Discussion",'{}'::text[]) AS "Discussion",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SiteDirectory"."SiteDirectoryDataAnnotation_Data"() AS "SiteDirectoryDataAnnotation" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid")
  LEFT JOIN (SELECT "SiteDirectoryThingReference"."Container" AS "Iid", array_agg("SiteDirectoryThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SiteDirectory"."SiteDirectoryThingReference_Data"() AS "SiteDirectoryThingReference"
   JOIN "SiteDirectory"."SiteDirectoryDataAnnotation_Data"() AS "SiteDirectoryDataAnnotation" ON "SiteDirectoryThingReference"."Container" = "SiteDirectoryDataAnnotation"."Iid"
   GROUP BY "SiteDirectoryThingReference"."Container") AS "SiteDirectoryDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "SiteDirectoryDataDiscussionItem"."Container" AS "Iid", array_agg("SiteDirectoryDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SiteDirectory"."SiteDirectoryDataDiscussionItem_Data"() AS "SiteDirectoryDataDiscussionItem"
   JOIN "SiteDirectory"."SiteDirectoryDataAnnotation_Data"() AS "SiteDirectoryDataAnnotation" ON "SiteDirectoryDataDiscussionItem"."Container" = "SiteDirectoryDataAnnotation"."Iid"
   GROUP BY "SiteDirectoryDataDiscussionItem"."Container") AS "SiteDirectoryDataAnnotation_Discussion" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."ThingReference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ThingReference"."ValueTypeDictionary" AS "ValueTypeSet",
	"ThingReference"."ReferencedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."ThingReference_Data"() AS "ThingReference" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SiteDirectoryThingReference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ThingReference"."ValueTypeDictionary" || "SiteDirectoryThingReference"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectoryThingReference"."Container",
	NULL::bigint AS "Sequence",
	"ThingReference"."ReferencedThing",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."ThingReference_Data"() AS "ThingReference" USING ("Iid")
  JOIN "SiteDirectory"."SiteDirectoryThingReference_Data"() AS "SiteDirectoryThingReference" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."DiscussionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "DiscussionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiscussionItem"."ReplyTo",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SiteDirectory"."DiscussionItem_Data"() AS "DiscussionItem" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");

CREATE OR REPLACE VIEW "SiteDirectory"."SiteDirectoryDataDiscussionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "DiscussionItem"."ValueTypeDictionary" || "SiteDirectoryDataDiscussionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectoryDataDiscussionItem"."Container",
	NULL::bigint AS "Sequence",
	"DiscussionItem"."ReplyTo",
	"SiteDirectoryDataDiscussionItem"."Author",
	COALESCE("Thing_Attachment"."Attachment",'{}'::text[]) AS "Attachment",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain"
  FROM "SiteDirectory"."Thing_Data"() AS "Thing"
  JOIN "SiteDirectory"."GenericAnnotation_Data"() AS "GenericAnnotation" USING ("Iid")
  JOIN "SiteDirectory"."DiscussionItem_Data"() AS "DiscussionItem" USING ("Iid")
  JOIN "SiteDirectory"."SiteDirectoryDataDiscussionItem_Data"() AS "SiteDirectoryDataDiscussionItem" USING ("Iid")
  LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedPerson"::text) AS "ExcludedPerson"
   FROM "SiteDirectory"."Thing_ExcludedPerson_Data"() AS "Thing_ExcludedPerson"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedPerson" USING ("Iid")
 LEFT JOIN (SELECT "Thing" AS "Iid", array_agg("ExcludedDomain"::text) AS "ExcludedDomain"
   FROM "SiteDirectory"."Thing_ExcludedDomain_Data"() AS "Thing_ExcludedDomain"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Thing" = "Iid"
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid")
  LEFT JOIN (SELECT "Attachment"."Container" AS "Iid", array_agg("Attachment"."Iid"::text) AS "Attachment"
   FROM "SiteDirectory"."Attachment_Data"() AS "Attachment"
   JOIN "SiteDirectory"."Thing_Data"() AS "Thing" ON "Attachment"."Container" = "Thing"."Iid"
   GROUP BY "Attachment"."Container") AS "Thing_Attachment" USING ("Iid");