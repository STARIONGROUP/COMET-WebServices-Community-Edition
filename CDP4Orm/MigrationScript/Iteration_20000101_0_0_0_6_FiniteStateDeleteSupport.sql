
SET CONSTRAINTS ALL DEFERRED;

-- use on delete cascade for ParameterValueSetBase actualoption and actualstate properties
ALTER TABLE "SchemaName_Replace"."ParameterValueSetBase" DROP CONSTRAINT "ParameterValueSetBase_FK_ActualOption";
ALTER TABLE "SchemaName_Replace"."ParameterValueSetBase" ADD CONSTRAINT "ParameterValueSetBase_FK_ActualOption" 
	FOREIGN KEY ("ActualOption") REFERENCES "SchemaName_Replace"."Option" ("Iid") MATCH SIMPLE
	ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "SchemaName_Replace"."ParameterValueSetBase" DROP CONSTRAINT "ParameterValueSetBase_FK_ActualState";
ALTER TABLE "SchemaName_Replace"."ParameterValueSetBase" ADD CONSTRAINT "ParameterValueSetBase_FK_ActualState" 
	FOREIGN KEY ("ActualState") REFERENCES "SchemaName_Replace"."ActualFiniteState" ("Iid") MATCH SIMPLE
    ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;


-- trigger function to also delete actual finite states when (any) referenced possible finite state is deleted (making sure it is not called on subsequent triggers)
-- see http://dba.stackexchange.com/questions/103402/how-to-prevent-a-postgresql-trigger-from-being-fired-by-another-trigger#103661
CREATE OR REPLACE FUNCTION "SchemaName_Replace".actualfinitestate_cleanup() RETURNS trigger
  LANGUAGE plpgsql
    AS $$
BEGIN
  DELETE FROM "SchemaName_Replace"."Thing"
  WHERE "Iid" = OLD."ActualFiniteState";
END;
$$;

DROP TRIGGER IF EXISTS actualfinitestate_possiblestate_cleanup ON "SchemaName_Replace"."ActualFiniteState_PossibleState";

CREATE TRIGGER actualfinitestate_possiblestate_cleanup
  AFTER DELETE ON "SchemaName_Replace"."ActualFiniteState_PossibleState"
  FOR EACH ROW 
  WHEN (pg_trigger_depth() < 1)
  EXECUTE PROCEDURE "SchemaName_Replace".actualfinitestate_cleanup();