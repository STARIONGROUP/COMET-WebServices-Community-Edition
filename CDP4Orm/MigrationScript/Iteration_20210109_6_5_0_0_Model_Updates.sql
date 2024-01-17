-- OrganizationalParticipant is a collection property (many to many) of class ElementDefinition: [0..*]-[1..1]
CREATE TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" (
  "ElementDefinition" uuid NOT NULL,
  "OrganizationalParticipant" uuid NOT NULL,
  "ValidFrom" timestamp NOT NULL DEFAULT "SiteDirectory".get_transaction_time(),
  "ValidTo" timestamp NOT NULL DEFAULT 'infinity',
  CONSTRAINT "ElementDefinition_OrganizationalParticipant_PK" PRIMARY KEY("ElementDefinition", "OrganizationalParticipant"),
  CONSTRAINT "ElementDefinition_OrganizationalParticipant_FK_Source" FOREIGN KEY ("ElementDefinition") REFERENCES "SchemaName_Replace"."ElementDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ElementDefinition_OrganizationalParticipant_FK_Target" FOREIGN KEY ("OrganizationalParticipant") REFERENCES "SiteDirectory"."OrganizationalParticipant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

CREATE INDEX "Idx_ElementDefinition_OrganizationalParticipant_ValidFrom" ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" ("ValidFrom");
CREATE INDEX "Idx_ElementDefinition_OrganizationalParticipant_ValidTo" ON "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" ("ValidTo");

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SchemaName_Replace"."ElementDefinition_OrganizationalParticipant" SET (autovacuum_analyze_threshold = 2500);  

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
