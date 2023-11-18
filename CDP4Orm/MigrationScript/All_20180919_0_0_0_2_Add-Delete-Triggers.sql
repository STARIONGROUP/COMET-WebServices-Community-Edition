CREATE OR REPLACE FUNCTION "SchemaName_Replace".delete_thing_record() RETURNS trigger
    LANGUAGE plpgsql 
    AS $$
BEGIN
  IF (TG_OP = 'DELETE') THEN
    EXECUTE 'DELETE FROM "SchemaName_Replace"."Thing" WHERE "Iid" = $1;' USING OLD."Iid";
  END IF;

  RETURN OLD;
END;
$$;