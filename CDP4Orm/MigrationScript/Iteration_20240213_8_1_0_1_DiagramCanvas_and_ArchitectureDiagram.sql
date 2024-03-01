-- DiagramCanvas
-- DiagramCanvas.LockedBy is an optional association to Person: [0..1]-[1..1]
ALTER TABLE "SchemaName_Replace"."DiagramCanvas" ADD COLUMN IF NOT EXISTS "LockedBy" uuid;
ALTER TABLE "SchemaName_Replace"."DiagramCanvas" DROP CONSTRAINT IF EXISTS "DiagramCanvas_FK_LockedBy";
ALTER TABLE "SchemaName_Replace"."DiagramCanvas" ADD CONSTRAINT "DiagramCanvas_FK_LockedBy" FOREIGN KEY ("LockedBy") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;

ALTER TABLE "SchemaName_Replace"."DiagramCanvas_Audit" 
  ADD COLUMN IF NOT EXISTS "LockedBy" uuid,
  ADD COLUMN "ActionNEW" character(1),
  ADD COLUMN "ActorNEW" uuid;

UPDATE "SchemaName_Replace"."DiagramCanvas_Audit" SET "ActionNEW" = "Action" WHERE 1=1;
UPDATE "SchemaName_Replace"."DiagramCanvas_Audit" SET "ActorNEW" = "Actor" WHERE 1=1;

ALTER TABLE "SchemaName_Replace"."DiagramCanvas_Audit"
  DROP COLUMN "Action",
  DROP COLUMN "Actor";

ALTER TABLE "SchemaName_Replace"."DiagramCanvas_Audit" 
  RENAME COLUMN "ActionNEW" TO "Action";

ALTER TABLE "SchemaName_Replace"."DiagramCanvas_Audit" 
  RENAME COLUMN "ActorNEW" TO "Actor";

ALTER TABLE "SchemaName_Replace"."DiagramCanvas_Audit" 
 ALTER COLUMN "Action" SET NOT NULL;