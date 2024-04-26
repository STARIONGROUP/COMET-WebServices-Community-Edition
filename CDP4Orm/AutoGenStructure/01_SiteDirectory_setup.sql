--------------------------------------------------------------------------------------------------------
-- Copyright Starion Group S.A.                                                                         --
--                                                                                                    --
-- Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Théate Antoine      --
--                                                                                                    --
-- This file is part of COMET Server Community Edition. The COMET Server Community Edition is the     --
-- STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.                                        --
--                                                                                                    --
-- The COMET Server Community Edition is free software; you can redistribute it and/or modify it      --
-- the terms of the GNU Affero General Public License as published by the Free Software Foundation;   --
-- either version 3 of the License, or (at your option) any later version.                            --
--                                                                                                    --
-- The COMET Server Community Edition is distributed in the hope that it will be useful, but WITHOUT  --
-- ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     --
-- PURPOSE. See the GNU Affero General Public License for more details.                               --
--                                                                                                    --
-- You should have received a copy of the GNU Affero General Public License                           --
-- along with this program.  If not, see <http://www.gnu.org/licenses/>.                              --
--------------------------------------------------------------------------------------------------------

--------------------------------------------------------------------------------------------------------
---------------------------------- Schema and Revision setup -------------------------------------------
--------------------------------------------------------------------------------------------------------

CREATE EXTENSION IF NOT EXISTS hstore;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE SCHEMA "SiteDirectory";
CREATE SEQUENCE "SiteDirectory"."Revision" MINVALUE 1 START 1;
CREATE TABLE "SiteDirectory"."RevisionRegistry"
(
  "Revision" integer NOT NULL,
  "Instant" timestamp NOT NULL,
  "Actor" uuid
);

ALTER TABLE "SiteDirectory"."RevisionRegistry" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RevisionRegistry" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "SiteDirectory"."RevisionRegistry" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "SiteDirectory"."RevisionRegistry" SET (autovacuum_analyze_threshold = 2500);

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.get_current_revision() --------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".get_current_revision() RETURNS INTEGER 
  LANGUAGE plpgsql
  AS $$
DECLARE
  transaction_time timestamp without time zone;
  revision integer;
  audit_enabled boolean;
  actor_id uuid;
BEGIN
  -- get the current transaction time
  transaction_time := "SiteDirectory".get_transaction_time();
  actor_id := "SiteDirectory".get_session_user();
  
  -- try and get the current revision
  SELECT "Revision" INTO revision FROM "SiteDirectory"."RevisionRegistry" WHERE "Instant" = transaction_time;
  
  IF(revision IS NULL) THEN
  
    -- no revision registry entry for this transaction; increase revision number
    SELECT nextval('"SiteDirectory"."Revision"') INTO revision;
    EXECUTE 'INSERT INTO "SiteDirectory"."RevisionRegistry" ("Revision", "Instant", "Actor") VALUES($1, $2, $3);' USING revision, transaction_time, actor_id;
  
    -- make sure to log the updated state of top container updates (even if audit logging is temporarily turned off)
    audit_enabled := "SiteDirectory".get_audit_enabled();
    IF (NOT audit_enabled) THEN
      -- enabled audit logging
      EXECUTE 'UPDATE transaction_info SET audit_enabled = true;';
    END IF;

    -- update the revision number and last modified on properties of the top container
    EXECUTE 'UPDATE "SiteDirectory"."TopContainer" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"LastModifiedOn" => "' || transaction_time || '"'';';
    EXECUTE 'UPDATE "SiteDirectory"."Thing" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"'' WHERE "Iid" = ANY(SELECT "Iid" FROM "SiteDirectory"."TopContainer");';
  
    IF (NOT audit_enabled) THEN
      -- turn off auditing again for remainder of transaction
      EXECUTE 'UPDATE transaction_info SET audit_enabled = false;';
    END IF;

  END IF;
  
  -- return the current revision number
  RETURN revision;
END;
$$;

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.revision_management() ---------------------------------
--------------------------------------------------------------------------------------------------------

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
  
    -- determine whether it is a move operation
    has_moved := NEW."Container" <> OLD."Container";
  
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

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.get_session_user() ------------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".get_session_user() RETURNS uuid
  LANGUAGE plpgsql
  AS $$
BEGIN
  IF EXISTS (SELECT column_name FROM information_schema.columns WHERE table_name='transaction_info' and column_name='user_id') THEN
    RETURN (SELECT user_id FROM transaction_info);
  ELSE
    RETURN NULL;
  END IF;
END;
$$;

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.get_session_timeframe_start()--------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".get_session_timeframe_start() RETURNS timestamp
  LANGUAGE plpgsql
  AS $$
BEGIN
  IF EXISTS (SELECT column_name FROM information_schema.columns WHERE table_name='transaction_info' and column_name='period_start') THEN
    RETURN (SELECT period_start FROM transaction_info);
  ELSE
    RETURN '-infinity';
  END IF;
END;
$$;

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.get_session_instant()----------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".get_session_instant() RETURNS timestamp
  LANGUAGE plpgsql
  AS $$
DECLARE
  session_info RECORD;
BEGIN
  IF (EXISTS (SELECT column_name FROM information_schema.columns WHERE table_name='transaction_info' and column_name='period_end') AND 
      EXISTS (SELECT column_name FROM information_schema.columns WHERE table_name='transaction_info' and column_name='instant')) THEN
  
    SELECT instant, period_end INTO session_info FROM transaction_info;
    
    IF session_info."instant" <= session_info."period_end" THEN
        RETURN session_info."instant";
    ELSE
        RETURN session_info."period_end";
    END IF;
  ELSE
    RETURN 'infinity';
  END IF;
END;
$$;

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.get_transaction_time ----------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".get_transaction_time() RETURNS timestamp
  LANGUAGE plpgsql
  AS $$
BEGIN
  IF EXISTS (SELECT column_name FROM information_schema.columns WHERE table_name='transaction_info' and column_name='transaction_time') THEN
    RETURN (SELECT transaction_time FROM transaction_info);
  ELSE
    RETURN statement_timestamp();
  END IF;
END;
$$;

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.get_audit_enabled -------------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".get_audit_enabled() RETURNS boolean
  LANGUAGE plpgsql
  AS $$
BEGIN
  IF EXISTS (SELECT column_name FROM information_schema.columns WHERE table_name='transaction_info' and column_name='audit_enabled') THEN
    RETURN (SELECT audit_enabled FROM transaction_info);
  ELSE
    RETURN 'true';
  END IF;
END;
$$;

--------------------------------------------------------------------------------------------------------
---------------------------------- SiteDirectory.process_timetravel_after ------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".process_timetravel_after() RETURNS trigger
  LANGUAGE plpgsql
  AS $_$
DECLARE
  temp_row RECORD; -- a temporary variable used on inserts/updates/deletes
  time_now TIMESTAMP;
  audit_cols VARCHAR;
  actor_id UUID; -- a temporary variable used to store the actor of the update
  action VARCHAR;
  audit_enabled Boolean;
BEGIN
  audit_enabled := "SiteDirectory".get_audit_enabled();
  time_now := "SiteDirectory".get_transaction_time();

  IF (TG_OP = 'INSERT' OR TG_OP = 'UPDATE' OR TG_OP = 'DELETE') THEN
    
    IF (TG_OP = 'INSERT') THEN
      temp_row := NEW;
      action := 'I';
    END IF;
    
    IF (TG_OP = 'UPDATE' AND audit_enabled) THEN
      temp_row := OLD;
      temp_row."ValidTo" := NEW."ValidFrom";
      action := 'U';
    END IF;
    
    IF (TG_OP = 'DELETE' AND audit_enabled) THEN
      temp_row := OLD;
      temp_row."ValidTo" := time_now;
      action := 'D';
    END IF;
    
    actor_id := "SiteDirectory".get_session_user();
    
    audit_cols := ', ''' || action || '''';
    
    IF(actor_id IS NOT NULL) THEN
      -- include session user as actor
      audit_cols := audit_cols || ', ''' || actor_id || '''::uuid';
    END IF;
    
    IF (TG_OP = 'INSERT' OR audit_enabled) THEN
      -- use TG_TABLE_SCHEMA and TG_TABLE_NAME to be generic and non-table specific
      EXECUTE 'INSERT INTO  "' || TG_TABLE_SCHEMA || '"."' || TG_TABLE_NAME || '_Audit" SELECT $1.*' || audit_cols USING temp_row;
    END IF;
    
  END IF;

  RETURN NULL; -- return value doesn't matter in after

END;
$_$;

--------------------------------------------------------------------------------------------------------
------------------------------ SiteDirectory.process_timetravel_before ---------------------------------
--------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION "SiteDirectory".process_timetravel_before() RETURNS trigger
  LANGUAGE plpgsql
  AS $$
DECLARE
  time_now TIMESTAMP;
  audit_enabled Boolean;
BEGIN
  audit_enabled := "SiteDirectory".get_audit_enabled();
  time_now = "SiteDirectory".get_transaction_time();

  IF (TG_OP = 'UPDATE') THEN
    
    IF (audit_enabled) THEN
      -- 'bump' the valid_from and ensure valid_to = 'infinity'
      NEW."ValidFrom" := time_now;
      NEW."ValidTo" := 'infinity';
    END IF;
    
    -- allow the update to continue, so that the correct number of rows are reported as having been affected
    RETURN NEW;
    
  END IF;

END;
$$;
