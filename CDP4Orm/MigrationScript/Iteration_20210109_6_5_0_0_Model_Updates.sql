-- OrganizationalParticipant is a collection property (many to many) of class ElementDefinition: [0..*]-[1..1]
CREATE TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" (
  "ElementDefinition" uuid NOT NULL,
  "OrganizationalParticipant" uuid NOT NULL,
  CONSTRAINT "ElementDefinition_OrganizationalParticipant_PK" PRIMARY KEY("ElementDefinition", "OrganizationalParticipant"),
  CONSTRAINT "ElementDefinition_OrganizationalParticipant_FK_Source" FOREIGN KEY ("ElementDefinition") REFERENCES "SchemaName_Replace"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ElementDefinition_OrganizationalParticipant_FK_Target" FOREIGN KEY ("OrganizationalParticipant") REFERENCES "SiteDirectory"."OrganizationalParticipant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ElementDefinition_OrganizationalParticipant_ValidFrom" ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinition_OrganizationalParticipant_ValidTo" ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" ("ValidTo");

CREATE TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" (LIKE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant");

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ElementDefinition_OrganizationalParticipantAudit_ValidFrom" ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinition_OrganizationalParticipantAudit_ValidTo" ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit" ("ValidTo");

CREATE TRIGGER ElementDefinition_OrganizationalParticipant_audit_prepare
  BEFORE UPDATE ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ElementDefinition_OrganizationalParticipant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER elementdefinition_organizationalparticipant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ElementDefinition', 'EngineeringModel_Replace');

CREATE OR REPLACE FUNCTION "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Data" ()
    RETURNS SETOF "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ElementDefinition","OrganizationalParticipant","ValidFrom","ValidTo" 
      FROM "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ElementDefinition","OrganizationalParticipant","ValidFrom","ValidTo"
      FROM "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

DROP VIEW "SchemaName_Replace"."ElementDefinition_View";

CREATE OR REPLACE VIEW "SchemaName_Replace"."ElementDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ElementBase"."ValueTypeDictionary" || "ElementDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"ElementDefinition"."Container",
	NULL::bigint AS "Sequence",
	"ElementBase"."Owner",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("ElementDefinition_ContainedElement"."ContainedElement",'{}'::text[]) AS "ContainedElement",
	COALESCE("ElementDefinition_Parameter"."Parameter",'{}'::text[]) AS "Parameter",
	COALESCE("ElementDefinition_ParameterGroup"."ParameterGroup",'{}'::text[]) AS "ParameterGroup",
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
   GROUP BY "ParameterGroup"."Container") AS "ElementDefinition_ParameterGroup" USING ("Iid");
