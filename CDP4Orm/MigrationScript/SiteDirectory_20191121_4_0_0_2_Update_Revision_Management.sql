
CREATE OR REPLACE FUNCTION "SiteDirectory".revision_management() RETURNS trigger
    LANGUAGE plpgsql 
    AS $$
DECLARE
  has_moved boolean;
  source_ref_name varchar;
  top_container_schema varchar;
  source_reference uuid;
  container_reference uuid;
  container_schema varchar;
  revision integer;
  valueDictionary hstore;
BEGIN
  source_ref_name := TG_ARGV[0];  
  has_moved := false;
  
  IF (TG_ARGV[1] IS NOT NULL) THEN  
    top_container_schema := TG_ARGV[1];
  ELSE
    top_container_schema := TG_TABLE_SCHEMA;
  END IF;

  container_schema := 'SKIP';
  
  -- if argument [2] is set use it as the container schema
  IF (TG_ARGV[2] IS NOT NULL) THEN  
    container_schema := TG_ARGV[2];
  END IF;
  
  -- use topcontainer_schema to get applicable revision (for this transaction)
  EXECUTE 'SELECT * FROM "' || top_container_schema || '".get_current_revision();' INTO revision;
  
  -- read source reference
  IF (TG_OP = 'UPDATE') THEN
  
    IF (hstore(NEW) ? 'Container') THEN

       -- determine whether it is a move operation
       has_moved := NEW."Container" <> OLD."Container";

	END IF;
  
    IF (source_ref_name = 'Container') THEN
      
      -- switch to Iid column to register updates on contained concepts
      source_ref_name := 'Iid';
  
    END IF;
  
    EXECUTE format('SELECT $1.%I', source_ref_name) INTO source_reference USING NEW;
  
  ELSEIF (TG_OP = 'INSERT') THEN

    IF (TG_TABLE_NAME = 'Thing') THEN
    
      EXECUTE 'SELECT $1."ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"''' INTO valueDictionary USING NEW;
      NEW."ValueTypeDictionary" := valueDictionary;
      RETURN NEW;
    
    ELSEIF (TG_TABLE_NAME = 'Iteration') THEN

      -- set the previous active (the only row that has the 'ToRevision' column set to NULL) iteration to closed
      EXECUTE 'UPDATE "' || TG_TABLE_SCHEMA || '"."IterationRevisionLog" SET "ToRevision" = "' || TG_TABLE_SCHEMA || '".get_current_revision() WHERE "ToRevision" IS NULL;';
      -- insert the new active iteration into the log
      EXECUTE 'INSERT INTO "' || TG_TABLE_SCHEMA || '"."IterationRevisionLog" ("IterationIid") VALUES ($1);' USING NEW."Iid";
    
    END IF;

    EXECUTE format('SELECT $1.%I', source_ref_name) INTO source_reference USING NEW;
    
    IF (container_schema <> 'SKIP') THEN
        EXECUTE format('SELECT $1.%I', 'Container') INTO container_reference USING NEW;
    END IF;
      
  ELSEIF (TG_OP = 'DELETE') THEN
  
    EXECUTE format('SELECT $1.%I', source_ref_name) INTO source_reference USING OLD;
    
    IF (container_schema <> 'SKIP') THEN
        EXECUTE format('SELECT $1.%I', 'Container') INTO container_reference USING OLD;
    END IF;
  
  END IF;
  
  -- set the revision number
  EXECUTE 'UPDATE "' || TG_TABLE_SCHEMA || '"."Thing" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"'' WHERE "Iid" = $1;' USING source_reference;
  
  IF (container_schema <> 'SKIP' AND (TG_OP = 'INSERT' OR TG_OP = 'DELETE')) THEN

    -- also update the direct container revision number after insert or delete
    EXECUTE 'UPDATE "' || container_schema || '"."Thing" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"'' WHERE "Iid" = $1;' USING container_reference;
    
  END IF;

  -- set a revision number to the containers of the moved thing
  IF (has_moved) THEN
	EXECUTE 'UPDATE "' || container_schema || '"."Thing" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"'' WHERE "Iid" = $1;' USING OLD."Container";
	EXECUTE 'UPDATE "' || container_schema || '"."Thing" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"'' WHERE "Iid" = $1;' USING NEW."Container";
  END IF;
  
  IF (TG_OP = 'UPDATE' OR TG_OP = 'INSERT') THEN
  
    -- allow the update to continue... so that the correct number of rows are reported as having been affected
    RETURN NEW;
  
  ELSEIF (TG_OP = 'DELETE') THEN
  
    -- allow the delete to continue... so that the correct number of rows are reported as having been affected
    RETURN OLD;
  
  END IF;

END;
$$;