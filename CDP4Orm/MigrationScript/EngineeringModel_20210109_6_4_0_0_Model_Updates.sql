-- Create table for class LogEntryChangelogItem (which derives from: Thing)
CREATE TABLE "SchemaName_Replace"."LogEntryChangelogItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "LogEntryChangelogItem_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for LogEntryChangelogItem
CREATE TABLE "SchemaName_Replace"."LogEntryChangelogItem_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogEntryChangelogItem_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for LogEntryChangelogItem
CREATE TABLE "SchemaName_Replace"."LogEntryChangelogItem_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogEntryChangelogItem_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "LogEntryChangelogItemCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Cache" SET (autovacuum_analyze_threshold = 2500);

-- AffectedDomainIid is a collection property of class ModelLogEntry: [0..*]
CREATE TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" (
  "ModelLogEntry" uuid NOT NULL,
  "AffectedDomainIid" uuid NOT NULL,
  CONSTRAINT "ModelLogEntry_AffectedDomainIid_PK" PRIMARY KEY("ModelLogEntry","AffectedDomainIid"),
  CONSTRAINT "ModelLogEntry_AffectedDomainIid_FK_Source" FOREIGN KEY ("ModelLogEntry") REFERENCES "SchemaName_Replace"."ModelLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ModelLogEntry_AffectedDomainIid_ValidFrom" ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_AffectedDomainIid_ValidTo" ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" (LIKE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid");

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModelLogEntry_AffectedDomainIidAudit_ValidFrom" ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModelLogEntry_AffectedDomainIidAudit_ValidTo" ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit" ("ValidTo");

CREATE TRIGGER ModelLogEntry_AffectedDomainIid_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelLogEntry_AffectedDomainIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER modellogentry_affecteddomainiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ModelLogEntry', 'SchemaName_Replace');
-- LogEntryChangelogItem is contained (composite) by ModelLogEntry: [0..*]-[1..1]
ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" ADD CONSTRAINT "LogEntryChangelogItem_FK_Container" FOREIGN KEY ("Container") REFERENCES "SchemaName_Replace"."ModelLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_LogEntryChangelogItem_Container" ON "SchemaName_Replace"."LogEntryChangelogItem" ("Container");
CREATE TRIGGER logentrychangelogitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."LogEntryChangelogItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SchemaName_Replace', 'SchemaName_Replace');
-- Class LogEntryChangelogItem derives from Thing
ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem" ADD CONSTRAINT "LogEntryChangelogItemDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SchemaName_Replace"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- AffectedReferenceIid is a collection property of class LogEntryChangelogItem: [0..*]
CREATE TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" (
  "LogEntryChangelogItem" uuid NOT NULL,
  "AffectedReferenceIid" uuid NOT NULL,
  CONSTRAINT "LogEntryChangelogItem_AffectedReferenceIid_PK" PRIMARY KEY("LogEntryChangelogItem","AffectedReferenceIid"),
  CONSTRAINT "LogEntryChangelogItem_AffectedReferenceIid_FK_Source" FOREIGN KEY ("LogEntryChangelogItem") REFERENCES "SchemaName_Replace"."LogEntryChangelogItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIid_ValidFrom" ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIid_ValidTo" ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" (LIKE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid");

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIidAudit_ValidFrom" ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItem_AffectedReferenceIidAudit_ValidTo" ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit" ("ValidTo");

CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_AffectedReferenceIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER logentrychangelogitem_affectedreferenceiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('LogEntryChangelogItem', 'SchemaName_Replace');

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_LogEntryChangelogItem_ValidFrom" ON "SchemaName_Replace"."LogEntryChangelogItem" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItem_ValidTo" ON "SchemaName_Replace"."LogEntryChangelogItem" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."LogEntryChangelogItem_Audit" (LIKE "SchemaName_Replace"."LogEntryChangelogItem");

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."LogEntryChangelogItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LogEntryChangelogItemAudit_ValidFrom" ON "SchemaName_Replace"."LogEntryChangelogItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_LogEntryChangelogItemAudit_ValidTo" ON "SchemaName_Replace"."LogEntryChangelogItem_Audit" ("ValidTo");

CREATE TRIGGER LogEntryChangelogItem_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogEntryChangelogItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."LogEntryChangelogItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."LogEntryChangelogItem_Data" ()
    RETURNS SETOF "SchemaName_Replace"."LogEntryChangelogItem" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."LogEntryChangelogItem";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."LogEntryChangelogItem"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."LogEntryChangelogItem_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Data" ()
    RETURNS SETOF "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ModelLogEntry","AffectedDomainIid","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ModelLogEntry","AffectedDomainIid","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."ModelLogEntry_AffectedDomainIid_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Data" ()
    RETURNS SETOF "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "LogEntryChangelogItem","AffectedReferenceIid","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "LogEntryChangelogItem","AffectedReferenceIid","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."LogEntryChangelogItem_AffectedReferenceIid_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

DROP VIEW "SchemaName_Replace"."ModelLogEntry_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ModelLogEntry_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ModelLogEntry"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModelLogEntry"."Container",
	NULL::bigint AS "Sequence",
	"ModelLogEntry"."Author",
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
  LEFT JOIN (SELECT "LogEntryChangelogItem"."Container" AS "Iid", array_agg("LogEntryChangelogItem"."Iid"::text) AS "LogEntryChangelogItem"
   FROM "SchemaName_Replace"."LogEntryChangelogItem_Data"() AS "LogEntryChangelogItem"
   JOIN "SchemaName_Replace"."ModelLogEntry_Data"() AS "ModelLogEntry" ON "LogEntryChangelogItem"."Container" = "ModelLogEntry"."Iid"
   GROUP BY "LogEntryChangelogItem"."Container") AS "ModelLogEntry_LogEntryChangelogItem" USING ("Iid");

CREATE VIEW "SchemaName_Replace"."LogEntryChangelogItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "LogEntryChangelogItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"LogEntryChangelogItem"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "LogEntryChangelogItem") AS "LogEntryChangelogItem_AffectedReferenceIid" USING ("Iid");
