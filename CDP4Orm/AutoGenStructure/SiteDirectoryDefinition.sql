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
-- Create table for class Thing
CREATE TABLE "SiteDirectory"."Thing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Thing_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing" SET (autovacuum_analyze_threshold = 2500);
CREATE TRIGGER thing_apply_revision
  BEFORE INSERT 
  ON "SiteDirectory"."Thing"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Iid', 'SiteDirectory', 'SiteDirectory');
-- Create table for class TopContainer (which derives from: Thing)
CREATE TABLE "SiteDirectory"."TopContainer" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "TopContainer_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TopContainer" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SiteDirectory (which derives from: TopContainer and implements: TimeStampedThing, NamedThing, ShortNamedThing)
CREATE TABLE "SiteDirectory"."SiteDirectory" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SiteDirectory_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SiteDirectory
CREATE TABLE "SiteDirectory"."SiteDirectory_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectory_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SiteDirectory
CREATE TABLE "SiteDirectory"."SiteDirectory_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectory_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Organization (which derives from: Thing and implements: NamedThing, ShortNamedThing, DeprecatableThing)
CREATE TABLE "SiteDirectory"."Organization" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Organization_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Organization
CREATE TABLE "SiteDirectory"."Organization_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Organization_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Organization
CREATE TABLE "SiteDirectory"."Organization_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Organization_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OrganizationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Person (which derives from: Thing and implements: ShortNamedThing, NamedThing, DeprecatableThing)
CREATE TABLE "SiteDirectory"."Person" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Person_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Person
CREATE TABLE "SiteDirectory"."Person_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Person_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Person
CREATE TABLE "SiteDirectory"."Person_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Person_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PersonCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class EmailAddress (which derives from: Thing)
CREATE TABLE "SiteDirectory"."EmailAddress" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "EmailAddress_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for EmailAddress
CREATE TABLE "SiteDirectory"."EmailAddress_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EmailAddress_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for EmailAddress
CREATE TABLE "SiteDirectory"."EmailAddress_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EmailAddress_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EmailAddressCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class TelephoneNumber (which derives from: Thing)
CREATE TABLE "SiteDirectory"."TelephoneNumber" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "TelephoneNumber_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for TelephoneNumber
CREATE TABLE "SiteDirectory"."TelephoneNumber_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TelephoneNumber_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for TelephoneNumber
CREATE TABLE "SiteDirectory"."TelephoneNumber_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TelephoneNumber_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TelephoneNumberCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class UserPreference (which derives from: Thing and implements: ShortNamedThing)
CREATE TABLE "SiteDirectory"."UserPreference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "UserPreference_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for UserPreference
CREATE TABLE "SiteDirectory"."UserPreference_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UserPreference_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for UserPreference
CREATE TABLE "SiteDirectory"."UserPreference_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UserPreference_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UserPreferenceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DefinedThing (which derives from: Thing and implements: NamedThing, ShortNamedThing)
CREATE TABLE "SiteDirectory"."DefinedThing" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DefinedThing_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DefinedThing" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ParticipantRole (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."ParticipantRole" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ParticipantRole_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ParticipantRole
CREATE TABLE "SiteDirectory"."ParticipantRole_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantRole_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ParticipantRole
CREATE TABLE "SiteDirectory"."ParticipantRole_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantRole_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParticipantRoleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Alias (which derives from: Thing and implements: Annotation)
CREATE TABLE "SiteDirectory"."Alias" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Alias_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Alias
CREATE TABLE "SiteDirectory"."Alias_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Alias_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Alias
CREATE TABLE "SiteDirectory"."Alias_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Alias_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "AliasCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Definition (which derives from: Thing and implements: Annotation)
CREATE TABLE "SiteDirectory"."Definition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Definition_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Definition
CREATE TABLE "SiteDirectory"."Definition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Definition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Definition
CREATE TABLE "SiteDirectory"."Definition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Definition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Citation (which derives from: Thing and implements: ShortNamedThing)
CREATE TABLE "SiteDirectory"."Citation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Citation_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Citation
CREATE TABLE "SiteDirectory"."Citation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Citation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Citation
CREATE TABLE "SiteDirectory"."Citation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Citation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CitationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class HyperLink (which derives from: Thing and implements: Annotation)
CREATE TABLE "SiteDirectory"."HyperLink" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "HyperLink_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for HyperLink
CREATE TABLE "SiteDirectory"."HyperLink_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "HyperLink_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for HyperLink
CREATE TABLE "SiteDirectory"."HyperLink_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "HyperLink_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "HyperLinkCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ParticipantPermission (which derives from: Thing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."ParticipantPermission" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ParticipantPermission_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ParticipantPermission
CREATE TABLE "SiteDirectory"."ParticipantPermission_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantPermission_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ParticipantPermission
CREATE TABLE "SiteDirectory"."ParticipantPermission_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParticipantPermission_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParticipantPermissionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ReferenceDataLibrary (which derives from: DefinedThing and implements: ParticipantAffectedAccessThing)
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ReferenceDataLibrary_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SiteReferenceDataLibrary (which derives from: ReferenceDataLibrary and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SiteReferenceDataLibrary_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SiteReferenceDataLibrary
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteReferenceDataLibrary_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SiteReferenceDataLibrary
CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteReferenceDataLibrary_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteReferenceDataLibraryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Category (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."Category" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Category_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Category
CREATE TABLE "SiteDirectory"."Category_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Category_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Category
CREATE TABLE "SiteDirectory"."Category_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Category_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CategoryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ParameterType (which derives from: DefinedThing and implements: DeprecatableThing, CategorizableThing)
CREATE TABLE "SiteDirectory"."ParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class CompoundParameterType (which derives from: ParameterType)
CREATE TABLE "SiteDirectory"."CompoundParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "CompoundParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for CompoundParameterType
CREATE TABLE "SiteDirectory"."CompoundParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CompoundParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for CompoundParameterType
CREATE TABLE "SiteDirectory"."CompoundParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CompoundParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CompoundParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ArrayParameterType (which derives from: CompoundParameterType)
CREATE TABLE "SiteDirectory"."ArrayParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ArrayParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ArrayParameterType
CREATE TABLE "SiteDirectory"."ArrayParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArrayParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ArrayParameterType
CREATE TABLE "SiteDirectory"."ArrayParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ArrayParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ArrayParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ParameterTypeComponent (which derives from: Thing and implements: ShortNamedThing)
CREATE TABLE "SiteDirectory"."ParameterTypeComponent" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ParameterTypeComponent_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ParameterTypeComponent
CREATE TABLE "SiteDirectory"."ParameterTypeComponent_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterTypeComponent_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ParameterTypeComponent
CREATE TABLE "SiteDirectory"."ParameterTypeComponent_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterTypeComponent_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterTypeComponentCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ScalarParameterType (which derives from: ParameterType)
CREATE TABLE "SiteDirectory"."ScalarParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ScalarParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScalarParameterType" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class EnumerationParameterType (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."EnumerationParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "EnumerationParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for EnumerationParameterType
CREATE TABLE "SiteDirectory"."EnumerationParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for EnumerationParameterType
CREATE TABLE "SiteDirectory"."EnumerationParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EnumerationParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class EnumerationValueDefinition (which derives from: DefinedThing)
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "EnumerationValueDefinition_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for EnumerationValueDefinition
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationValueDefinition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for EnumerationValueDefinition
CREATE TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EnumerationValueDefinition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EnumerationValueDefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class BooleanParameterType (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."BooleanParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "BooleanParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for BooleanParameterType
CREATE TABLE "SiteDirectory"."BooleanParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BooleanParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for BooleanParameterType
CREATE TABLE "SiteDirectory"."BooleanParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BooleanParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BooleanParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DateParameterType (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."DateParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DateParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DateParameterType
CREATE TABLE "SiteDirectory"."DateParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DateParameterType
CREATE TABLE "SiteDirectory"."DateParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DateParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class TextParameterType (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."TextParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "TextParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for TextParameterType
CREATE TABLE "SiteDirectory"."TextParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TextParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for TextParameterType
CREATE TABLE "SiteDirectory"."TextParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TextParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TextParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DateTimeParameterType (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."DateTimeParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DateTimeParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DateTimeParameterType
CREATE TABLE "SiteDirectory"."DateTimeParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateTimeParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DateTimeParameterType
CREATE TABLE "SiteDirectory"."DateTimeParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DateTimeParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DateTimeParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class TimeOfDayParameterType (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "TimeOfDayParameterType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for TimeOfDayParameterType
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TimeOfDayParameterType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for TimeOfDayParameterType
CREATE TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "TimeOfDayParameterType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TimeOfDayParameterTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class QuantityKind (which derives from: ScalarParameterType)
CREATE TABLE "SiteDirectory"."QuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "QuantityKind_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SpecializedQuantityKind (which derives from: QuantityKind)
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SpecializedQuantityKind_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SpecializedQuantityKind
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SpecializedQuantityKind_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SpecializedQuantityKind
CREATE TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SpecializedQuantityKind_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SpecializedQuantityKindCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SimpleQuantityKind (which derives from: QuantityKind)
CREATE TABLE "SiteDirectory"."SimpleQuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SimpleQuantityKind_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SimpleQuantityKind
CREATE TABLE "SiteDirectory"."SimpleQuantityKind_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleQuantityKind_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SimpleQuantityKind
CREATE TABLE "SiteDirectory"."SimpleQuantityKind_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleQuantityKind_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SimpleQuantityKindCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DerivedQuantityKind (which derives from: QuantityKind)
CREATE TABLE "SiteDirectory"."DerivedQuantityKind" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DerivedQuantityKind_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DerivedQuantityKind
CREATE TABLE "SiteDirectory"."DerivedQuantityKind_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedQuantityKind_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DerivedQuantityKind
CREATE TABLE "SiteDirectory"."DerivedQuantityKind_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedQuantityKind_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DerivedQuantityKindCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class QuantityKindFactor (which derives from: Thing)
CREATE TABLE "SiteDirectory"."QuantityKindFactor" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "QuantityKindFactor_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for QuantityKindFactor
CREATE TABLE "SiteDirectory"."QuantityKindFactor_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "QuantityKindFactor_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for QuantityKindFactor
CREATE TABLE "SiteDirectory"."QuantityKindFactor_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "QuantityKindFactor_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "QuantityKindFactorCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class MeasurementScale (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."MeasurementScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "MeasurementScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementScale" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class OrdinalScale (which derives from: MeasurementScale)
CREATE TABLE "SiteDirectory"."OrdinalScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "OrdinalScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for OrdinalScale
CREATE TABLE "SiteDirectory"."OrdinalScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrdinalScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for OrdinalScale
CREATE TABLE "SiteDirectory"."OrdinalScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "OrdinalScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "OrdinalScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ScaleValueDefinition (which derives from: DefinedThing)
CREATE TABLE "SiteDirectory"."ScaleValueDefinition" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ScaleValueDefinition_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ScaleValueDefinition
CREATE TABLE "SiteDirectory"."ScaleValueDefinition_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleValueDefinition_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ScaleValueDefinition
CREATE TABLE "SiteDirectory"."ScaleValueDefinition_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleValueDefinition_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ScaleValueDefinitionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class MappingToReferenceScale (which derives from: Thing)
CREATE TABLE "SiteDirectory"."MappingToReferenceScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "MappingToReferenceScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for MappingToReferenceScale
CREATE TABLE "SiteDirectory"."MappingToReferenceScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MappingToReferenceScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for MappingToReferenceScale
CREATE TABLE "SiteDirectory"."MappingToReferenceScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MappingToReferenceScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "MappingToReferenceScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class RatioScale (which derives from: MeasurementScale)
CREATE TABLE "SiteDirectory"."RatioScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "RatioScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for RatioScale
CREATE TABLE "SiteDirectory"."RatioScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RatioScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for RatioScale
CREATE TABLE "SiteDirectory"."RatioScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "RatioScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "RatioScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class CyclicRatioScale (which derives from: RatioScale)
CREATE TABLE "SiteDirectory"."CyclicRatioScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "CyclicRatioScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for CyclicRatioScale
CREATE TABLE "SiteDirectory"."CyclicRatioScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CyclicRatioScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for CyclicRatioScale
CREATE TABLE "SiteDirectory"."CyclicRatioScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "CyclicRatioScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "CyclicRatioScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class IntervalScale (which derives from: MeasurementScale)
CREATE TABLE "SiteDirectory"."IntervalScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "IntervalScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for IntervalScale
CREATE TABLE "SiteDirectory"."IntervalScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IntervalScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for IntervalScale
CREATE TABLE "SiteDirectory"."IntervalScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IntervalScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IntervalScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class LogarithmicScale (which derives from: MeasurementScale)
CREATE TABLE "SiteDirectory"."LogarithmicScale" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "LogarithmicScale_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for LogarithmicScale
CREATE TABLE "SiteDirectory"."LogarithmicScale_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogarithmicScale_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for LogarithmicScale
CREATE TABLE "SiteDirectory"."LogarithmicScale_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LogarithmicScale_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "LogarithmicScaleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ScaleReferenceQuantityValue (which derives from: Thing)
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ScaleReferenceQuantityValue_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ScaleReferenceQuantityValue
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleReferenceQuantityValue_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ScaleReferenceQuantityValue
CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ScaleReferenceQuantityValue_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ScaleReferenceQuantityValueCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class UnitPrefix (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."UnitPrefix" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "UnitPrefix_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for UnitPrefix
CREATE TABLE "SiteDirectory"."UnitPrefix_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitPrefix_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for UnitPrefix
CREATE TABLE "SiteDirectory"."UnitPrefix_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitPrefix_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UnitPrefixCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class MeasurementUnit (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."MeasurementUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "MeasurementUnit_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementUnit" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DerivedUnit (which derives from: MeasurementUnit)
CREATE TABLE "SiteDirectory"."DerivedUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DerivedUnit_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DerivedUnit
CREATE TABLE "SiteDirectory"."DerivedUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DerivedUnit
CREATE TABLE "SiteDirectory"."DerivedUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DerivedUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DerivedUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class UnitFactor (which derives from: Thing)
CREATE TABLE "SiteDirectory"."UnitFactor" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "UnitFactor_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for UnitFactor
CREATE TABLE "SiteDirectory"."UnitFactor_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitFactor_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for UnitFactor
CREATE TABLE "SiteDirectory"."UnitFactor_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "UnitFactor_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "UnitFactorCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ConversionBasedUnit (which derives from: MeasurementUnit)
CREATE TABLE "SiteDirectory"."ConversionBasedUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ConversionBasedUnit_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class LinearConversionUnit (which derives from: ConversionBasedUnit)
CREATE TABLE "SiteDirectory"."LinearConversionUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "LinearConversionUnit_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for LinearConversionUnit
CREATE TABLE "SiteDirectory"."LinearConversionUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LinearConversionUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for LinearConversionUnit
CREATE TABLE "SiteDirectory"."LinearConversionUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "LinearConversionUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "LinearConversionUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class PrefixedUnit (which derives from: ConversionBasedUnit)
CREATE TABLE "SiteDirectory"."PrefixedUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "PrefixedUnit_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for PrefixedUnit
CREATE TABLE "SiteDirectory"."PrefixedUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PrefixedUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for PrefixedUnit
CREATE TABLE "SiteDirectory"."PrefixedUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PrefixedUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PrefixedUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SimpleUnit (which derives from: MeasurementUnit)
CREATE TABLE "SiteDirectory"."SimpleUnit" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SimpleUnit_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SimpleUnit
CREATE TABLE "SiteDirectory"."SimpleUnit_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleUnit_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SimpleUnit
CREATE TABLE "SiteDirectory"."SimpleUnit_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SimpleUnit_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SimpleUnitCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class FileType (which derives from: DefinedThing and implements: CategorizableThing, DeprecatableThing)
CREATE TABLE "SiteDirectory"."FileType" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "FileType_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for FileType
CREATE TABLE "SiteDirectory"."FileType_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileType_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for FileType
CREATE TABLE "SiteDirectory"."FileType_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "FileType_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "FileTypeCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Glossary (which derives from: DefinedThing and implements: CategorizableThing, DeprecatableThing)
CREATE TABLE "SiteDirectory"."Glossary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Glossary_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Glossary
CREATE TABLE "SiteDirectory"."Glossary_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Glossary_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Glossary
CREATE TABLE "SiteDirectory"."Glossary_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Glossary_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "GlossaryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Term (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."Term" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Term_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Term
CREATE TABLE "SiteDirectory"."Term_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Term_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Term
CREATE TABLE "SiteDirectory"."Term_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Term_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "TermCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ReferenceSource (which derives from: DefinedThing and implements: CategorizableThing, DeprecatableThing)
CREATE TABLE "SiteDirectory"."ReferenceSource" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ReferenceSource_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ReferenceSource
CREATE TABLE "SiteDirectory"."ReferenceSource_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferenceSource_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ReferenceSource
CREATE TABLE "SiteDirectory"."ReferenceSource_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferenceSource_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ReferenceSourceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Rule (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."Rule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Rule_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Rule" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ReferencerRule (which derives from: Rule)
CREATE TABLE "SiteDirectory"."ReferencerRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ReferencerRule_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ReferencerRule
CREATE TABLE "SiteDirectory"."ReferencerRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferencerRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ReferencerRule
CREATE TABLE "SiteDirectory"."ReferencerRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ReferencerRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ReferencerRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class BinaryRelationshipRule (which derives from: Rule)
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "BinaryRelationshipRule_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for BinaryRelationshipRule
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryRelationshipRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for BinaryRelationshipRule
CREATE TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "BinaryRelationshipRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "BinaryRelationshipRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class MultiRelationshipRule (which derives from: Rule)
CREATE TABLE "SiteDirectory"."MultiRelationshipRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "MultiRelationshipRule_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for MultiRelationshipRule
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MultiRelationshipRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for MultiRelationshipRule
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "MultiRelationshipRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "MultiRelationshipRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DecompositionRule (which derives from: Rule)
CREATE TABLE "SiteDirectory"."DecompositionRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DecompositionRule_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DecompositionRule
CREATE TABLE "SiteDirectory"."DecompositionRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DecompositionRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DecompositionRule
CREATE TABLE "SiteDirectory"."DecompositionRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DecompositionRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DecompositionRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ParameterizedCategoryRule (which derives from: Rule)
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ParameterizedCategoryRule_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ParameterizedCategoryRule
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterizedCategoryRule_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ParameterizedCategoryRule
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ParameterizedCategoryRule_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParameterizedCategoryRuleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Constant (which derives from: DefinedThing and implements: DeprecatableThing, CategorizableThing)
CREATE TABLE "SiteDirectory"."Constant" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Constant_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Constant
CREATE TABLE "SiteDirectory"."Constant_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Constant_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Constant
CREATE TABLE "SiteDirectory"."Constant_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Constant_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ConstantCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class EngineeringModelSetup (which derives from: DefinedThing and implements: ParticipantAffectedAccessThing)
CREATE TABLE "SiteDirectory"."EngineeringModelSetup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "EngineeringModelSetup_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for EngineeringModelSetup
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelSetup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for EngineeringModelSetup
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "EngineeringModelSetup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "EngineeringModelSetupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class Participant (which derives from: Thing and implements: ParticipantAffectedAccessThing)
CREATE TABLE "SiteDirectory"."Participant" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "Participant_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for Participant
CREATE TABLE "SiteDirectory"."Participant_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Participant_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for Participant
CREATE TABLE "SiteDirectory"."Participant_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "Participant_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ParticipantCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ModelReferenceDataLibrary (which derives from: ReferenceDataLibrary)
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ModelReferenceDataLibrary_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for ModelReferenceDataLibrary
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModelReferenceDataLibrary_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for ModelReferenceDataLibrary
CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "ModelReferenceDataLibrary_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "ModelReferenceDataLibraryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class IterationSetup (which derives from: Thing and implements: TimeStampedThing, ParticipantAffectedAccessThing)
CREATE TABLE "SiteDirectory"."IterationSetup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "IterationSetup_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for IterationSetup
CREATE TABLE "SiteDirectory"."IterationSetup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IterationSetup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for IterationSetup
CREATE TABLE "SiteDirectory"."IterationSetup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "IterationSetup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "IterationSetupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup_Cache" SET (autovacuum_analyze_threshold = 2500);

-- Create table for class PersonRole (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."PersonRole" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "PersonRole_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for PersonRole
CREATE TABLE "SiteDirectory"."PersonRole_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonRole_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for PersonRole
CREATE TABLE "SiteDirectory"."PersonRole_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonRole_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PersonRoleCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class PersonPermission (which derives from: Thing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."PersonPermission" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "PersonPermission_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for PersonPermission
CREATE TABLE "SiteDirectory"."PersonPermission_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonPermission_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for PersonPermission
CREATE TABLE "SiteDirectory"."PersonPermission_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "PersonPermission_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "PersonPermissionCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SiteLogEntry (which derives from: Thing and implements: TimeStampedThing, Annotation, CategorizableThing, LogEntry)
CREATE TABLE "SiteDirectory"."SiteLogEntry" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SiteLogEntry_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SiteLogEntry
CREATE TABLE "SiteDirectory"."SiteLogEntry_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteLogEntry_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SiteLogEntry
CREATE TABLE "SiteDirectory"."SiteLogEntry_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteLogEntry_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteLogEntryCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DomainOfExpertiseGroup (which derives from: DefinedThing and implements: DeprecatableThing)
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DomainOfExpertiseGroup_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DomainOfExpertiseGroup
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertiseGroup_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DomainOfExpertiseGroup
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertiseGroup_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DomainOfExpertiseGroupCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DomainOfExpertise (which derives from: DefinedThing and implements: DeprecatableThing, CategorizableThing)
CREATE TABLE "SiteDirectory"."DomainOfExpertise" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DomainOfExpertise_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for DomainOfExpertise
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertise_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for DomainOfExpertise
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "DomainOfExpertise_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "DomainOfExpertiseCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class NaturalLanguage (which derives from: Thing and implements: NamedThing)
CREATE TABLE "SiteDirectory"."NaturalLanguage" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "NaturalLanguage_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for NaturalLanguage
CREATE TABLE "SiteDirectory"."NaturalLanguage_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NaturalLanguage_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for NaturalLanguage
CREATE TABLE "SiteDirectory"."NaturalLanguage_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "NaturalLanguage_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "NaturalLanguageCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class GenericAnnotation (which derives from: Thing and implements: Annotation, TimeStampedThing)
CREATE TABLE "SiteDirectory"."GenericAnnotation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "GenericAnnotation_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."GenericAnnotation" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SiteDirectoryDataAnnotation (which derives from: GenericAnnotation)
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SiteDirectoryDataAnnotation_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SiteDirectoryDataAnnotation
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataAnnotation_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SiteDirectoryDataAnnotation
CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataAnnotation_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryDataAnnotationCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class ThingReference (which derives from: Thing)
CREATE TABLE "SiteDirectory"."ThingReference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "ThingReference_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ThingReference" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SiteDirectoryThingReference (which derives from: ThingReference)
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SiteDirectoryThingReference_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SiteDirectoryThingReference
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryThingReference_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SiteDirectoryThingReference
CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryThingReference_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryThingReferenceCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Cache" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class DiscussionItem (which derives from: GenericAnnotation)
CREATE TABLE "SiteDirectory"."DiscussionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "DiscussionItem_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DiscussionItem" SET (autovacuum_analyze_threshold = 2500);
-- Create table for class SiteDirectoryDataDiscussionItem (which derives from: DiscussionItem)
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" (
  "Iid" uuid NOT NULL,
  "ValueTypeDictionary" hstore NOT NULL DEFAULT ''::hstore,
  CONSTRAINT "SiteDirectoryDataDiscussionItem_PK" PRIMARY KEY ("Iid")
);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" SET (autovacuum_analyze_threshold = 2500);
-- create revision-history table for SiteDirectoryDataDiscussionItem
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Instant" timestamp without time zone NOT NULL,
  "Actor" uuid,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataDiscussionItem_REV_PK" PRIMARY KEY ("Iid", "RevisionNumber")
);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Revision" SET (autovacuum_analyze_threshold = 2500);
-- create cache table for SiteDirectoryDataDiscussionItem
CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" (
  "Iid" uuid NOT NULL,
  "RevisionNumber" integer NOT NULL,
  "Jsonb" jsonb NOT NULL,
  CONSTRAINT "SiteDirectoryDataDiscussionItem_CACHE_PK" PRIMARY KEY ("Iid"),
  CONSTRAINT "SiteDirectoryDataDiscussionItemCacheDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") MATCH SIMPLE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Cache" SET (autovacuum_analyze_threshold = 2500);
-- ExcludedPerson is a collection property (many to many) of class Thing: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."Thing_ExcludedPerson" (
  "Thing" uuid NOT NULL,
  "ExcludedPerson" uuid NOT NULL,
  CONSTRAINT "Thing_ExcludedPerson_PK" PRIMARY KEY("Thing", "ExcludedPerson"),
  CONSTRAINT "Thing_ExcludedPerson_FK_Source" FOREIGN KEY ("Thing") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedPerson_FK_Target" FOREIGN KEY ("ExcludedPerson") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidFrom" ON "SiteDirectory"."Thing_ExcludedPerson" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPerson_ValidTo" ON "SiteDirectory"."Thing_ExcludedPerson" ("ValidTo");

CREATE TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" (LIKE "SiteDirectory"."Thing_ExcludedPerson");

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_ExcludedPerson_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidFrom" ON "SiteDirectory"."Thing_ExcludedPerson_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedPersonAudit_ValidTo" ON "SiteDirectory"."Thing_ExcludedPerson_Audit" ("ValidTo");

CREATE TRIGGER Thing_ExcludedPerson_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedPerson_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Thing_ExcludedPerson"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER thing_excludedperson_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Thing_ExcludedPerson"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'SiteDirectory');
-- ExcludedDomain is a collection property (many to many) of class Thing: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."Thing_ExcludedDomain" (
  "Thing" uuid NOT NULL,
  "ExcludedDomain" uuid NOT NULL,
  CONSTRAINT "Thing_ExcludedDomain_PK" PRIMARY KEY("Thing", "ExcludedDomain"),
  CONSTRAINT "Thing_ExcludedDomain_FK_Source" FOREIGN KEY ("Thing") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Thing_ExcludedDomain_FK_Target" FOREIGN KEY ("ExcludedDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidFrom" ON "SiteDirectory"."Thing_ExcludedDomain" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomain_ValidTo" ON "SiteDirectory"."Thing_ExcludedDomain" ("ValidTo");

CREATE TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" (LIKE "SiteDirectory"."Thing_ExcludedDomain");

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_ExcludedDomain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidFrom" ON "SiteDirectory"."Thing_ExcludedDomain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Thing_ExcludedDomainAudit_ValidTo" ON "SiteDirectory"."Thing_ExcludedDomain_Audit" ("ValidTo");

CREATE TRIGGER Thing_ExcludedDomain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_ExcludedDomain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Thing_ExcludedDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER thing_excludeddomain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Thing_ExcludedDomain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Thing', 'SiteDirectory');
-- Class TopContainer derives from Thing
ALTER TABLE "SiteDirectory"."TopContainer" ADD CONSTRAINT "TopContainerDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class SiteDirectory derives from TopContainer
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD CONSTRAINT "SiteDirectoryDerivesFromTopContainer" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."TopContainer" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Organization is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Organization" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Organization" ADD CONSTRAINT "Organization_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Organization_Container" ON "SiteDirectory"."Organization" ("Container");
CREATE TRIGGER organization_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Organization"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Person is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Person_Container" ON "SiteDirectory"."Person" ("Container");
CREATE TRIGGER person_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Person"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- ParticipantRole is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParticipantRole" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParticipantRole" ADD CONSTRAINT "ParticipantRole_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ParticipantRole_Container" ON "SiteDirectory"."ParticipantRole" ("Container");
CREATE TRIGGER participantrole_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParticipantRole"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- SiteDirectory.DefaultParticipantRole is an optional association to ParticipantRole: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD COLUMN "DefaultParticipantRole" uuid;
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD CONSTRAINT "SiteDirectory_FK_DefaultParticipantRole" FOREIGN KEY ("DefaultParticipantRole") REFERENCES "SiteDirectory"."ParticipantRole" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- SiteReferenceDataLibrary is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" ADD CONSTRAINT "SiteReferenceDataLibrary_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_SiteReferenceDataLibrary_Container" ON "SiteDirectory"."SiteReferenceDataLibrary" ("Container");
CREATE TRIGGER sitereferencedatalibrary_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteReferenceDataLibrary"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- EngineeringModelSetup is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD CONSTRAINT "EngineeringModelSetup_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_EngineeringModelSetup_Container" ON "SiteDirectory"."EngineeringModelSetup" ("Container");
CREATE TRIGGER engineeringmodelsetup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EngineeringModelSetup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- PersonRole is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."PersonRole" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."PersonRole" ADD CONSTRAINT "PersonRole_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_PersonRole_Container" ON "SiteDirectory"."PersonRole" ("Container");
CREATE TRIGGER personrole_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."PersonRole"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- SiteDirectory.DefaultPersonRole is an optional association to PersonRole: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD COLUMN "DefaultPersonRole" uuid;
ALTER TABLE "SiteDirectory"."SiteDirectory" ADD CONSTRAINT "SiteDirectory_FK_DefaultPersonRole" FOREIGN KEY ("DefaultPersonRole") REFERENCES "SiteDirectory"."PersonRole" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- SiteLogEntry is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD CONSTRAINT "SiteLogEntry_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_SiteLogEntry_Container" ON "SiteDirectory"."SiteLogEntry" ("Container");
CREATE TRIGGER sitelogentry_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- DomainOfExpertiseGroup is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" ADD CONSTRAINT "DomainOfExpertiseGroup_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_DomainOfExpertiseGroup_Container" ON "SiteDirectory"."DomainOfExpertiseGroup" ("Container");
CREATE TRIGGER domainofexpertisegroup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertiseGroup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- DomainOfExpertise is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."DomainOfExpertise" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DomainOfExpertise" ADD CONSTRAINT "DomainOfExpertise_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_DomainOfExpertise_Container" ON "SiteDirectory"."DomainOfExpertise" ("Container");
CREATE TRIGGER domainofexpertise_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertise"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- NaturalLanguage is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."NaturalLanguage" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."NaturalLanguage" ADD CONSTRAINT "NaturalLanguage_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_NaturalLanguage_Container" ON "SiteDirectory"."NaturalLanguage" ("Container");
CREATE TRIGGER naturallanguage_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."NaturalLanguage"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- SiteDirectoryDataAnnotation is contained (composite) by SiteDirectory: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotation_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectory" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_SiteDirectoryDataAnnotation_Container" ON "SiteDirectory"."SiteDirectoryDataAnnotation" ("Container");
CREATE TRIGGER sitedirectorydataannotation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteDirectoryDataAnnotation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class Organization derives from Thing
ALTER TABLE "SiteDirectory"."Organization" ADD CONSTRAINT "OrganizationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class Person derives from Thing
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "PersonDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Person.Organization is an optional association to Organization: [0..1]-[0..*]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "Organization" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_Organization" FOREIGN KEY ("Organization") REFERENCES "SiteDirectory"."Organization" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- EmailAddress is contained (composite) by Person: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."EmailAddress" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."EmailAddress" ADD CONSTRAINT "EmailAddress_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_EmailAddress_Container" ON "SiteDirectory"."EmailAddress" ("Container");
CREATE TRIGGER emailaddress_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EmailAddress"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- TelephoneNumber is contained (composite) by Person: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."TelephoneNumber" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."TelephoneNumber" ADD CONSTRAINT "TelephoneNumber_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_TelephoneNumber_Container" ON "SiteDirectory"."TelephoneNumber" ("Container");
CREATE TRIGGER telephonenumber_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."TelephoneNumber"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Person.DefaultDomain is an optional association to DomainOfExpertise: [0..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "DefaultDomain" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_DefaultDomain" FOREIGN KEY ("DefaultDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Person.Role is an optional association to PersonRole: [0..1]-[0..*]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "Role" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_Role" FOREIGN KEY ("Role") REFERENCES "SiteDirectory"."PersonRole" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Person.DefaultEmailAddress is an optional association to EmailAddress: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "DefaultEmailAddress" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_DefaultEmailAddress" FOREIGN KEY ("DefaultEmailAddress") REFERENCES "SiteDirectory"."EmailAddress" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Person.DefaultTelephoneNumber is an optional association to TelephoneNumber: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."Person" ADD COLUMN "DefaultTelephoneNumber" uuid;
ALTER TABLE "SiteDirectory"."Person" ADD CONSTRAINT "Person_FK_DefaultTelephoneNumber" FOREIGN KEY ("DefaultTelephoneNumber") REFERENCES "SiteDirectory"."TelephoneNumber" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- UserPreference is contained (composite) by Person: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."UserPreference" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UserPreference" ADD CONSTRAINT "UserPreference_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_UserPreference_Container" ON "SiteDirectory"."UserPreference" ("Container");
CREATE TRIGGER userpreference_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."UserPreference"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class EmailAddress derives from Thing
ALTER TABLE "SiteDirectory"."EmailAddress" ADD CONSTRAINT "EmailAddressDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class TelephoneNumber derives from Thing
ALTER TABLE "SiteDirectory"."TelephoneNumber" ADD CONSTRAINT "TelephoneNumberDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- VcardType is a collection property of class TelephoneNumber: [0..*]
CREATE TABLE "SiteDirectory"."TelephoneNumber_VcardType" (
  "TelephoneNumber" uuid NOT NULL,
  "VcardType" text NOT NULL,
  CONSTRAINT "TelephoneNumber_VcardType_PK" PRIMARY KEY("TelephoneNumber","VcardType"),
  CONSTRAINT "TelephoneNumber_VcardType_FK_Source" FOREIGN KEY ("TelephoneNumber") REFERENCES "SiteDirectory"."TelephoneNumber" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_TelephoneNumber_VcardType_ValidFrom" ON "SiteDirectory"."TelephoneNumber_VcardType" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumber_VcardType_ValidTo" ON "SiteDirectory"."TelephoneNumber_VcardType" ("ValidTo");

CREATE TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" (LIKE "SiteDirectory"."TelephoneNumber_VcardType");

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_VcardType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TelephoneNumber_VcardTypeAudit_ValidFrom" ON "SiteDirectory"."TelephoneNumber_VcardType_Audit" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumber_VcardTypeAudit_ValidTo" ON "SiteDirectory"."TelephoneNumber_VcardType_Audit" ("ValidTo");

CREATE TRIGGER TelephoneNumber_VcardType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TelephoneNumber_VcardType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TelephoneNumber_VcardType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TelephoneNumber_VcardType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER telephonenumber_vcardtype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."TelephoneNumber_VcardType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('TelephoneNumber', 'SiteDirectory');
-- Class UserPreference derives from Thing
ALTER TABLE "SiteDirectory"."UserPreference" ADD CONSTRAINT "UserPreferenceDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DefinedThing derives from Thing
ALTER TABLE "SiteDirectory"."DefinedThing" ADD CONSTRAINT "DefinedThingDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Alias is contained (composite) by DefinedThing: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Alias" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Alias" ADD CONSTRAINT "Alias_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Alias_Container" ON "SiteDirectory"."Alias" ("Container");
CREATE TRIGGER alias_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Alias"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Definition is contained (composite) by DefinedThing: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Definition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Definition" ADD CONSTRAINT "Definition_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Definition_Container" ON "SiteDirectory"."Definition" ("Container");
CREATE TRIGGER definition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Definition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- HyperLink is contained (composite) by DefinedThing: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."HyperLink" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."HyperLink" ADD CONSTRAINT "HyperLink_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_HyperLink_Container" ON "SiteDirectory"."HyperLink" ("Container");
CREATE TRIGGER hyperlink_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."HyperLink"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class ParticipantRole derives from DefinedThing
ALTER TABLE "SiteDirectory"."ParticipantRole" ADD CONSTRAINT "ParticipantRoleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ParticipantPermission is contained (composite) by ParticipantRole: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParticipantPermission" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParticipantPermission" ADD CONSTRAINT "ParticipantPermission_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ParticipantRole" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ParticipantPermission_Container" ON "SiteDirectory"."ParticipantPermission" ("Container");
CREATE TRIGGER participantpermission_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParticipantPermission"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class Alias derives from Thing
ALTER TABLE "SiteDirectory"."Alias" ADD CONSTRAINT "AliasDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class Definition derives from Thing
ALTER TABLE "SiteDirectory"."Definition" ADD CONSTRAINT "DefinitionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Citation is contained (composite) by Definition: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Citation" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Citation" ADD CONSTRAINT "Citation_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Citation_Container" ON "SiteDirectory"."Citation" ("Container");
CREATE TRIGGER citation_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Citation"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Note is an ordered collection property of class Definition: [0..*] (ordered)
CREATE TABLE "SiteDirectory"."Definition_Note" (
  "Definition" uuid NOT NULL,
  "Note" text NOT NULL,
  "Sequence" bigint NOT NULL,
  CONSTRAINT "Definition_Note_PK" PRIMARY KEY("Definition","Note"),
  CONSTRAINT "Definition_Note_FK_Source" FOREIGN KEY ("Definition") REFERENCES "SiteDirectory"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Note" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."Definition_Note"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Definition_Note_ValidFrom" ON "SiteDirectory"."Definition_Note" ("ValidFrom");
CREATE INDEX "Idx_Definition_Note_ValidTo" ON "SiteDirectory"."Definition_Note" ("ValidTo");

CREATE TABLE "SiteDirectory"."Definition_Note_Audit" (LIKE "SiteDirectory"."Definition_Note");

ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Note_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Note_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Definition_NoteAudit_ValidFrom" ON "SiteDirectory"."Definition_Note_Audit" ("ValidFrom");
CREATE INDEX "Idx_Definition_NoteAudit_ValidTo" ON "SiteDirectory"."Definition_Note_Audit" ("ValidTo");

CREATE TRIGGER Definition_Note_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Definition_Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_Note_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Definition_Note"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER definition_note_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Definition_Note"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Definition', 'SiteDirectory');
-- Example is an ordered collection property of class Definition: [0..*] (ordered)
CREATE TABLE "SiteDirectory"."Definition_Example" (
  "Definition" uuid NOT NULL,
  "Example" text NOT NULL,
  "Sequence" bigint NOT NULL,
  CONSTRAINT "Definition_Example_PK" PRIMARY KEY("Definition","Example"),
  CONSTRAINT "Definition_Example_FK_Source" FOREIGN KEY ("Definition") REFERENCES "SiteDirectory"."Definition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Example" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."Definition_Example"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Definition_Example_ValidFrom" ON "SiteDirectory"."Definition_Example" ("ValidFrom");
CREATE INDEX "Idx_Definition_Example_ValidTo" ON "SiteDirectory"."Definition_Example" ("ValidTo");

CREATE TABLE "SiteDirectory"."Definition_Example_Audit" (LIKE "SiteDirectory"."Definition_Example");

ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Example_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Example_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Definition_ExampleAudit_ValidFrom" ON "SiteDirectory"."Definition_Example_Audit" ("ValidFrom");
CREATE INDEX "Idx_Definition_ExampleAudit_ValidTo" ON "SiteDirectory"."Definition_Example_Audit" ("ValidTo");

CREATE TRIGGER Definition_Example_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Definition_Example"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_Example_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Definition_Example"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER definition_example_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Definition_Example"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Definition', 'SiteDirectory');
-- Class Citation derives from Thing
ALTER TABLE "SiteDirectory"."Citation" ADD CONSTRAINT "CitationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Citation.Source is an association to ReferenceSource: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Citation" ADD COLUMN "Source" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Citation" ADD CONSTRAINT "Citation_FK_Source" FOREIGN KEY ("Source") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class HyperLink derives from Thing
ALTER TABLE "SiteDirectory"."HyperLink" ADD CONSTRAINT "HyperLinkDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ParticipantPermission derives from Thing
ALTER TABLE "SiteDirectory"."ParticipantPermission" ADD CONSTRAINT "ParticipantPermissionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ReferenceDataLibrary derives from DefinedThing
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" ADD CONSTRAINT "ReferenceDataLibraryDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Category is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Category" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Category" ADD CONSTRAINT "Category_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Category_Container" ON "SiteDirectory"."Category" ("Container");
CREATE TRIGGER category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- ParameterType is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParameterType" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterType" ADD CONSTRAINT "ParameterType_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ParameterType_Container" ON "SiteDirectory"."ParameterType" ("Container");
CREATE TRIGGER parametertype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- BaseQuantityKind is an ordered collection property (many to many) of class ReferenceDataLibrary: [0..*]-[1..1] (ordered)
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" (
  "ReferenceDataLibrary" uuid NOT NULL,
  "BaseQuantityKind" uuid NOT NULL,
  "Sequence" bigint NOT NULL,
  CONSTRAINT "ReferenceDataLibrary_BaseQuantityKind_PK" PRIMARY KEY("ReferenceDataLibrary", "BaseQuantityKind"),
  CONSTRAINT "ReferenceDataLibrary_BaseQuantityKind_FK_Source" FOREIGN KEY ("ReferenceDataLibrary") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferenceDataLibrary_BaseQuantityKind_FK_Target" FOREIGN KEY ("BaseQuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKind_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKind_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" (LIKE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind");

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKindAudit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseQuantityKindAudit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit" ("ValidTo");

CREATE TRIGGER ReferenceDataLibrary_BaseQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceDataLibrary_BaseQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER referencedatalibrary_basequantitykind_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferenceDataLibrary', 'SiteDirectory');
-- MeasurementScale is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD CONSTRAINT "MeasurementScale_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_MeasurementScale_Container" ON "SiteDirectory"."MeasurementScale" ("Container");
CREATE TRIGGER measurementscale_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MeasurementScale"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- UnitPrefix is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."UnitPrefix" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UnitPrefix" ADD CONSTRAINT "UnitPrefix_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_UnitPrefix_Container" ON "SiteDirectory"."UnitPrefix" ("Container");
CREATE TRIGGER unitprefix_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."UnitPrefix"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- MeasurementUnit is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."MeasurementUnit" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MeasurementUnit" ADD CONSTRAINT "MeasurementUnit_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_MeasurementUnit_Container" ON "SiteDirectory"."MeasurementUnit" ("Container");
CREATE TRIGGER measurementunit_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MeasurementUnit"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- BaseUnit is a collection property (many to many) of class ReferenceDataLibrary: [0..*]-[1..1]
CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" (
  "ReferenceDataLibrary" uuid NOT NULL,
  "BaseUnit" uuid NOT NULL,
  CONSTRAINT "ReferenceDataLibrary_BaseUnit_PK" PRIMARY KEY("ReferenceDataLibrary", "BaseUnit"),
  CONSTRAINT "ReferenceDataLibrary_BaseUnit_FK_Source" FOREIGN KEY ("ReferenceDataLibrary") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferenceDataLibrary_BaseUnit_FK_Target" FOREIGN KEY ("BaseUnit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" (LIKE "SiteDirectory"."ReferenceDataLibrary_BaseUnit");

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnitAudit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_BaseUnitAudit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit" ("ValidTo");

CREATE TRIGGER ReferenceDataLibrary_BaseUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceDataLibrary_BaseUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER referencedatalibrary_baseunit_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferenceDataLibrary', 'SiteDirectory');
-- FileType is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."FileType" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."FileType" ADD CONSTRAINT "FileType_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_FileType_Container" ON "SiteDirectory"."FileType" ("Container");
CREATE TRIGGER filetype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."FileType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Glossary is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Glossary" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Glossary" ADD CONSTRAINT "Glossary_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Glossary_Container" ON "SiteDirectory"."Glossary" ("Container");
CREATE TRIGGER glossary_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Glossary"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- ReferenceSource is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSource_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ReferenceSource_Container" ON "SiteDirectory"."ReferenceSource" ("Container");
CREATE TRIGGER referencesource_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceSource"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Rule is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Rule" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Rule" ADD CONSTRAINT "Rule_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Rule_Container" ON "SiteDirectory"."Rule" ("Container");
CREATE TRIGGER rule_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Rule"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- ReferenceDataLibrary.RequiredRdl is an optional association to SiteReferenceDataLibrary: [0..1]-[1..*]
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" ADD COLUMN "RequiredRdl" uuid;
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary" ADD CONSTRAINT "ReferenceDataLibrary_FK_RequiredRdl" FOREIGN KEY ("RequiredRdl") REFERENCES "SiteDirectory"."SiteReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Constant is contained (composite) by ReferenceDataLibrary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Constant" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "Constant_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Constant_Container" ON "SiteDirectory"."Constant" ("Container");
CREATE TRIGGER constant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Constant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class SiteReferenceDataLibrary derives from ReferenceDataLibrary
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary" ADD CONSTRAINT "SiteReferenceDataLibraryDerivesFromReferenceDataLibrary" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class Category derives from DefinedThing
ALTER TABLE "SiteDirectory"."Category" ADD CONSTRAINT "CategoryDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- SuperCategory is a collection property (many to many) of class Category: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Category_SuperCategory" (
  "Category" uuid NOT NULL,
  "SuperCategory" uuid NOT NULL,
  CONSTRAINT "Category_SuperCategory_PK" PRIMARY KEY("Category", "SuperCategory"),
  CONSTRAINT "Category_SuperCategory_FK_Source" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Category_SuperCategory_FK_Target" FOREIGN KEY ("SuperCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_SuperCategory" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Category_SuperCategory"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Category_SuperCategory_ValidFrom" ON "SiteDirectory"."Category_SuperCategory" ("ValidFrom");
CREATE INDEX "Idx_Category_SuperCategory_ValidTo" ON "SiteDirectory"."Category_SuperCategory" ("ValidTo");

CREATE TABLE "SiteDirectory"."Category_SuperCategory_Audit" (LIKE "SiteDirectory"."Category_SuperCategory");

ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_SuperCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Category_SuperCategoryAudit_ValidFrom" ON "SiteDirectory"."Category_SuperCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_Category_SuperCategoryAudit_ValidTo" ON "SiteDirectory"."Category_SuperCategory_Audit" ("ValidTo");

CREATE TRIGGER Category_SuperCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Category_SuperCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Category_SuperCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Category_SuperCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER category_supercategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Category_SuperCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Category', 'SiteDirectory');
-- PermissibleClass is a collection property of class Category: [1..*]
CREATE TABLE "SiteDirectory"."Category_PermissibleClass" (
  "Category" uuid NOT NULL,
  "PermissibleClass" text NOT NULL,
  CONSTRAINT "Category_PermissibleClass_PK" PRIMARY KEY("Category","PermissibleClass"),
  CONSTRAINT "Category_PermissibleClass_FK_Source" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."Category_PermissibleClass"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Category_PermissibleClass_ValidFrom" ON "SiteDirectory"."Category_PermissibleClass" ("ValidFrom");
CREATE INDEX "Idx_Category_PermissibleClass_ValidTo" ON "SiteDirectory"."Category_PermissibleClass" ("ValidTo");

CREATE TABLE "SiteDirectory"."Category_PermissibleClass_Audit" (LIKE "SiteDirectory"."Category_PermissibleClass");

ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_PermissibleClass_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Category_PermissibleClassAudit_ValidFrom" ON "SiteDirectory"."Category_PermissibleClass_Audit" ("ValidFrom");
CREATE INDEX "Idx_Category_PermissibleClassAudit_ValidTo" ON "SiteDirectory"."Category_PermissibleClass_Audit" ("ValidTo");

CREATE TRIGGER Category_PermissibleClass_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Category_PermissibleClass"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Category_PermissibleClass_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Category_PermissibleClass"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER category_permissibleclass_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Category_PermissibleClass"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Category', 'SiteDirectory');
-- Class ParameterType derives from DefinedThing
ALTER TABLE "SiteDirectory"."ParameterType" ADD CONSTRAINT "ParameterTypeDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Category is a collection property (many to many) of class ParameterType: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."ParameterType_Category" (
  "ParameterType" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "ParameterType_Category_PK" PRIMARY KEY("ParameterType", "Category"),
  CONSTRAINT "ParameterType_Category_FK_Source" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ParameterType_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."ParameterType_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParameterType_Category_ValidFrom" ON "SiteDirectory"."ParameterType_Category" ("ValidFrom");
CREATE INDEX "Idx_ParameterType_Category_ValidTo" ON "SiteDirectory"."ParameterType_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParameterType_Category_Audit" (LIKE "SiteDirectory"."ParameterType_Category");

ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterType_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterType_CategoryAudit_ValidFrom" ON "SiteDirectory"."ParameterType_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterType_CategoryAudit_ValidTo" ON "SiteDirectory"."ParameterType_Category_Audit" ("ValidTo");

CREATE TRIGGER ParameterType_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterType_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER parametertype_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterType_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ParameterType', 'SiteDirectory');
-- Class CompoundParameterType derives from ParameterType
ALTER TABLE "SiteDirectory"."CompoundParameterType" ADD CONSTRAINT "CompoundParameterTypeDerivesFromParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ParameterTypeComponent is contained (composite) by CompoundParameterType: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponent_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."CompoundParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ParameterTypeComponent_Container" ON "SiteDirectory"."ParameterTypeComponent" ("Container");
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "Sequence" bigint NOT NULL;
CREATE TRIGGER parametertypecomponent_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterTypeComponent"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class ArrayParameterType derives from CompoundParameterType
ALTER TABLE "SiteDirectory"."ArrayParameterType" ADD CONSTRAINT "ArrayParameterTypeDerivesFromCompoundParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."CompoundParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Dimension is an ordered collection property of class ArrayParameterType: [1..*] (ordered)
CREATE TABLE "SiteDirectory"."ArrayParameterType_Dimension" (
  "ArrayParameterType" uuid NOT NULL,
  "Dimension" integer NOT NULL,
  "Sequence" bigint NOT NULL,
  CONSTRAINT "ArrayParameterType_Dimension_FK_Source" FOREIGN KEY ("ArrayParameterType") REFERENCES "SiteDirectory"."ArrayParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ArrayParameterType_Dimension_ValidFrom" ON "SiteDirectory"."ArrayParameterType_Dimension" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterType_Dimension_ValidTo" ON "SiteDirectory"."ArrayParameterType_Dimension" ("ValidTo");

CREATE TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" (LIKE "SiteDirectory"."ArrayParameterType_Dimension");

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Dimension_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ArrayParameterType_DimensionAudit_ValidFrom" ON "SiteDirectory"."ArrayParameterType_Dimension_Audit" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterType_DimensionAudit_ValidTo" ON "SiteDirectory"."ArrayParameterType_Dimension_Audit" ("ValidTo");

CREATE TRIGGER ArrayParameterType_Dimension_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ArrayParameterType_Dimension"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArrayParameterType_Dimension_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ArrayParameterType_Dimension"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER arrayparametertype_dimension_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ArrayParameterType_Dimension"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ArrayParameterType', 'SiteDirectory');
-- Class ParameterTypeComponent derives from Thing
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponentDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ParameterTypeComponent.ParameterType is an association to ParameterType: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponent_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ParameterTypeComponent.Scale is an optional association to MeasurementScale: [0..1]-[0..*]
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD COLUMN "Scale" uuid;
ALTER TABLE "SiteDirectory"."ParameterTypeComponent" ADD CONSTRAINT "ParameterTypeComponent_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Class ScalarParameterType derives from ParameterType
ALTER TABLE "SiteDirectory"."ScalarParameterType" ADD CONSTRAINT "ScalarParameterTypeDerivesFromParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class EnumerationParameterType derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."EnumerationParameterType" ADD CONSTRAINT "EnumerationParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- EnumerationValueDefinition is contained (composite) by EnumerationParameterType: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD CONSTRAINT "EnumerationValueDefinition_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EnumerationParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_EnumerationValueDefinition_Container" ON "SiteDirectory"."EnumerationValueDefinition" ("Container");
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD COLUMN "Sequence" bigint NOT NULL;
CREATE TRIGGER enumerationvaluedefinition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EnumerationValueDefinition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class EnumerationValueDefinition derives from DefinedThing
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition" ADD CONSTRAINT "EnumerationValueDefinitionDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class BooleanParameterType derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."BooleanParameterType" ADD CONSTRAINT "BooleanParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DateParameterType derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."DateParameterType" ADD CONSTRAINT "DateParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class TextParameterType derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."TextParameterType" ADD CONSTRAINT "TextParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DateTimeParameterType derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."DateTimeParameterType" ADD CONSTRAINT "DateTimeParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class TimeOfDayParameterType derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType" ADD CONSTRAINT "TimeOfDayParameterTypeDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class QuantityKind derives from ScalarParameterType
ALTER TABLE "SiteDirectory"."QuantityKind" ADD CONSTRAINT "QuantityKindDerivesFromScalarParameterType" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ScalarParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- PossibleScale is a collection property (many to many) of class QuantityKind: [0..*]-[1..*]
CREATE TABLE "SiteDirectory"."QuantityKind_PossibleScale" (
  "QuantityKind" uuid NOT NULL,
  "PossibleScale" uuid NOT NULL,
  CONSTRAINT "QuantityKind_PossibleScale_PK" PRIMARY KEY("QuantityKind", "PossibleScale"),
  CONSTRAINT "QuantityKind_PossibleScale_FK_Source" FOREIGN KEY ("QuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "QuantityKind_PossibleScale_FK_Target" FOREIGN KEY ("PossibleScale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_QuantityKind_PossibleScale_ValidFrom" ON "SiteDirectory"."QuantityKind_PossibleScale" ("ValidFrom");
CREATE INDEX "Idx_QuantityKind_PossibleScale_ValidTo" ON "SiteDirectory"."QuantityKind_PossibleScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" (LIKE "SiteDirectory"."QuantityKind_PossibleScale");

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKind_PossibleScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_QuantityKind_PossibleScaleAudit_ValidFrom" ON "SiteDirectory"."QuantityKind_PossibleScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_QuantityKind_PossibleScaleAudit_ValidTo" ON "SiteDirectory"."QuantityKind_PossibleScale_Audit" ("ValidTo");

CREATE TRIGGER QuantityKind_PossibleScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."QuantityKind_PossibleScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER QuantityKind_PossibleScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."QuantityKind_PossibleScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER quantitykind_possiblescale_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."QuantityKind_PossibleScale"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('QuantityKind', 'SiteDirectory');
-- QuantityKind.DefaultScale is an association to MeasurementScale: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."QuantityKind" ADD COLUMN "DefaultScale" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."QuantityKind" ADD CONSTRAINT "QuantityKind_FK_DefaultScale" FOREIGN KEY ("DefaultScale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class SpecializedQuantityKind derives from QuantityKind
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" ADD CONSTRAINT "SpecializedQuantityKindDerivesFromQuantityKind" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- SpecializedQuantityKind.General is an association to QuantityKind: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" ADD COLUMN "General" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind" ADD CONSTRAINT "SpecializedQuantityKind_FK_General" FOREIGN KEY ("General") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class SimpleQuantityKind derives from QuantityKind
ALTER TABLE "SiteDirectory"."SimpleQuantityKind" ADD CONSTRAINT "SimpleQuantityKindDerivesFromQuantityKind" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DerivedQuantityKind derives from QuantityKind
ALTER TABLE "SiteDirectory"."DerivedQuantityKind" ADD CONSTRAINT "DerivedQuantityKindDerivesFromQuantityKind" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- QuantityKindFactor is contained (composite) by DerivedQuantityKind: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD CONSTRAINT "QuantityKindFactor_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DerivedQuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_QuantityKindFactor_Container" ON "SiteDirectory"."QuantityKindFactor" ("Container");
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD COLUMN "Sequence" bigint NOT NULL;
CREATE TRIGGER quantitykindfactor_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."QuantityKindFactor"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class QuantityKindFactor derives from Thing
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD CONSTRAINT "QuantityKindFactorDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- QuantityKindFactor.QuantityKind is an association to QuantityKind: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD COLUMN "QuantityKind" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."QuantityKindFactor" ADD CONSTRAINT "QuantityKindFactor_FK_QuantityKind" FOREIGN KEY ("QuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class MeasurementScale derives from DefinedThing
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD CONSTRAINT "MeasurementScaleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- MeasurementScale.Unit is an association to MeasurementUnit: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD COLUMN "Unit" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MeasurementScale" ADD CONSTRAINT "MeasurementScale_FK_Unit" FOREIGN KEY ("Unit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ScaleValueDefinition is contained (composite) by MeasurementScale: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" ADD CONSTRAINT "ScaleValueDefinition_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ScaleValueDefinition_Container" ON "SiteDirectory"."ScaleValueDefinition" ("Container");
CREATE TRIGGER scalevaluedefinition_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ScaleValueDefinition"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- MappingToReferenceScale is contained (composite) by MeasurementScale: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScale_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_MappingToReferenceScale_Container" ON "SiteDirectory"."MappingToReferenceScale" ("Container");
CREATE TRIGGER mappingtoreferencescale_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MappingToReferenceScale"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class OrdinalScale derives from MeasurementScale
ALTER TABLE "SiteDirectory"."OrdinalScale" ADD CONSTRAINT "OrdinalScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ScaleValueDefinition derives from DefinedThing
ALTER TABLE "SiteDirectory"."ScaleValueDefinition" ADD CONSTRAINT "ScaleValueDefinitionDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class MappingToReferenceScale derives from Thing
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScaleDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- MappingToReferenceScale.ReferenceScaleValue is an association to ScaleValueDefinition: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD COLUMN "ReferenceScaleValue" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScale_FK_ReferenceScaleValue" FOREIGN KEY ("ReferenceScaleValue") REFERENCES "SiteDirectory"."ScaleValueDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- MappingToReferenceScale.DependentScaleValue is an association to ScaleValueDefinition: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD COLUMN "DependentScaleValue" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MappingToReferenceScale" ADD CONSTRAINT "MappingToReferenceScale_FK_DependentScaleValue" FOREIGN KEY ("DependentScaleValue") REFERENCES "SiteDirectory"."ScaleValueDefinition" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class RatioScale derives from MeasurementScale
ALTER TABLE "SiteDirectory"."RatioScale" ADD CONSTRAINT "RatioScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class CyclicRatioScale derives from RatioScale
ALTER TABLE "SiteDirectory"."CyclicRatioScale" ADD CONSTRAINT "CyclicRatioScaleDerivesFromRatioScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."RatioScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class IntervalScale derives from MeasurementScale
ALTER TABLE "SiteDirectory"."IntervalScale" ADD CONSTRAINT "IntervalScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class LogarithmicScale derives from MeasurementScale
ALTER TABLE "SiteDirectory"."LogarithmicScale" ADD CONSTRAINT "LogarithmicScaleDerivesFromMeasurementScale" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- LogarithmicScale.ReferenceQuantityKind is an association to QuantityKind: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."LogarithmicScale" ADD COLUMN "ReferenceQuantityKind" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."LogarithmicScale" ADD CONSTRAINT "LogarithmicScale_FK_ReferenceQuantityKind" FOREIGN KEY ("ReferenceQuantityKind") REFERENCES "SiteDirectory"."QuantityKind" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ScaleReferenceQuantityValue is contained (composite) by LogarithmicScale: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD CONSTRAINT "ScaleReferenceQuantityValue_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."LogarithmicScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ScaleReferenceQuantityValue_Container" ON "SiteDirectory"."ScaleReferenceQuantityValue" ("Container");
CREATE TRIGGER scalereferencequantityvalue_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ScaleReferenceQuantityValue"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class ScaleReferenceQuantityValue derives from Thing
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD CONSTRAINT "ScaleReferenceQuantityValueDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ScaleReferenceQuantityValue.Scale is an association to MeasurementScale: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD COLUMN "Scale" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue" ADD CONSTRAINT "ScaleReferenceQuantityValue_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class UnitPrefix derives from DefinedThing
ALTER TABLE "SiteDirectory"."UnitPrefix" ADD CONSTRAINT "UnitPrefixDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class MeasurementUnit derives from DefinedThing
ALTER TABLE "SiteDirectory"."MeasurementUnit" ADD CONSTRAINT "MeasurementUnitDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DerivedUnit derives from MeasurementUnit
ALTER TABLE "SiteDirectory"."DerivedUnit" ADD CONSTRAINT "DerivedUnitDerivesFromMeasurementUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- UnitFactor is contained (composite) by DerivedUnit: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."UnitFactor" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UnitFactor" ADD CONSTRAINT "UnitFactor_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."DerivedUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_UnitFactor_Container" ON "SiteDirectory"."UnitFactor" ("Container");
ALTER TABLE "SiteDirectory"."UnitFactor" ADD COLUMN "Sequence" bigint NOT NULL;
CREATE TRIGGER unitfactor_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."UnitFactor"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class UnitFactor derives from Thing
ALTER TABLE "SiteDirectory"."UnitFactor" ADD CONSTRAINT "UnitFactorDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- UnitFactor.Unit is an association to MeasurementUnit: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."UnitFactor" ADD COLUMN "Unit" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."UnitFactor" ADD CONSTRAINT "UnitFactor_FK_Unit" FOREIGN KEY ("Unit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ConversionBasedUnit derives from MeasurementUnit
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" ADD CONSTRAINT "ConversionBasedUnitDerivesFromMeasurementUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ConversionBasedUnit.ReferenceUnit is an association to MeasurementUnit: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" ADD COLUMN "ReferenceUnit" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ConversionBasedUnit" ADD CONSTRAINT "ConversionBasedUnit_FK_ReferenceUnit" FOREIGN KEY ("ReferenceUnit") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class LinearConversionUnit derives from ConversionBasedUnit
ALTER TABLE "SiteDirectory"."LinearConversionUnit" ADD CONSTRAINT "LinearConversionUnitDerivesFromConversionBasedUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ConversionBasedUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class PrefixedUnit derives from ConversionBasedUnit
ALTER TABLE "SiteDirectory"."PrefixedUnit" ADD CONSTRAINT "PrefixedUnitDerivesFromConversionBasedUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ConversionBasedUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- PrefixedUnit.Prefix is an association to UnitPrefix: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."PrefixedUnit" ADD COLUMN "Prefix" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."PrefixedUnit" ADD CONSTRAINT "PrefixedUnit_FK_Prefix" FOREIGN KEY ("Prefix") REFERENCES "SiteDirectory"."UnitPrefix" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class SimpleUnit derives from MeasurementUnit
ALTER TABLE "SiteDirectory"."SimpleUnit" ADD CONSTRAINT "SimpleUnitDerivesFromMeasurementUnit" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."MeasurementUnit" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class FileType derives from DefinedThing
ALTER TABLE "SiteDirectory"."FileType" ADD CONSTRAINT "FileTypeDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Category is a collection property (many to many) of class FileType: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."FileType_Category" (
  "FileType" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "FileType_Category_PK" PRIMARY KEY("FileType", "Category"),
  CONSTRAINT "FileType_Category_FK_Source" FOREIGN KEY ("FileType") REFERENCES "SiteDirectory"."FileType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "FileType_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."FileType_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_FileType_Category_ValidFrom" ON "SiteDirectory"."FileType_Category" ("ValidFrom");
CREATE INDEX "Idx_FileType_Category_ValidTo" ON "SiteDirectory"."FileType_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."FileType_Category_Audit" (LIKE "SiteDirectory"."FileType_Category");

ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileType_CategoryAudit_ValidFrom" ON "SiteDirectory"."FileType_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileType_CategoryAudit_ValidTo" ON "SiteDirectory"."FileType_Category_Audit" ("ValidTo");

CREATE TRIGGER FileType_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."FileType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileType_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."FileType_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER filetype_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."FileType_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('FileType', 'SiteDirectory');
-- Class Glossary derives from DefinedThing
ALTER TABLE "SiteDirectory"."Glossary" ADD CONSTRAINT "GlossaryDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Term is contained (composite) by Glossary: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."Term" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Term" ADD CONSTRAINT "Term_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."Glossary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Term_Container" ON "SiteDirectory"."Term" ("Container");
CREATE TRIGGER term_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Term"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Category is a collection property (many to many) of class Glossary: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Glossary_Category" (
  "Glossary" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "Glossary_Category_PK" PRIMARY KEY("Glossary", "Category"),
  CONSTRAINT "Glossary_Category_FK_Source" FOREIGN KEY ("Glossary") REFERENCES "SiteDirectory"."Glossary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Glossary_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Glossary_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Glossary_Category_ValidFrom" ON "SiteDirectory"."Glossary_Category" ("ValidFrom");
CREATE INDEX "Idx_Glossary_Category_ValidTo" ON "SiteDirectory"."Glossary_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."Glossary_Category_Audit" (LIKE "SiteDirectory"."Glossary_Category");

ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Glossary_CategoryAudit_ValidFrom" ON "SiteDirectory"."Glossary_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Glossary_CategoryAudit_ValidTo" ON "SiteDirectory"."Glossary_Category_Audit" ("ValidTo");

CREATE TRIGGER Glossary_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Glossary_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Glossary_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Glossary_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER glossary_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Glossary_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Glossary', 'SiteDirectory');
-- Class Term derives from DefinedThing
ALTER TABLE "SiteDirectory"."Term" ADD CONSTRAINT "TermDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ReferenceSource derives from DefinedThing
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSourceDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ReferenceSource.Publisher is an optional association to Organization: [0..1]
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD COLUMN "Publisher" uuid;
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSource_FK_Publisher" FOREIGN KEY ("Publisher") REFERENCES "SiteDirectory"."Organization" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- ReferenceSource.PublishedIn is an optional association to ReferenceSource: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD COLUMN "PublishedIn" uuid;
ALTER TABLE "SiteDirectory"."ReferenceSource" ADD CONSTRAINT "ReferenceSource_FK_PublishedIn" FOREIGN KEY ("PublishedIn") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Category is a collection property (many to many) of class ReferenceSource: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."ReferenceSource_Category" (
  "ReferenceSource" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "ReferenceSource_Category_PK" PRIMARY KEY("ReferenceSource", "Category"),
  CONSTRAINT "ReferenceSource_Category_FK_Source" FOREIGN KEY ("ReferenceSource") REFERENCES "SiteDirectory"."ReferenceSource" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferenceSource_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."ReferenceSource_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferenceSource_Category_ValidFrom" ON "SiteDirectory"."ReferenceSource_Category" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSource_Category_ValidTo" ON "SiteDirectory"."ReferenceSource_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferenceSource_Category_Audit" (LIKE "SiteDirectory"."ReferenceSource_Category");

ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceSource_CategoryAudit_ValidFrom" ON "SiteDirectory"."ReferenceSource_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSource_CategoryAudit_ValidTo" ON "SiteDirectory"."ReferenceSource_Category_Audit" ("ValidTo");

CREATE TRIGGER ReferenceSource_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceSource_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceSource_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceSource_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER referencesource_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferenceSource_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferenceSource', 'SiteDirectory');
-- Class Rule derives from DefinedThing
ALTER TABLE "SiteDirectory"."Rule" ADD CONSTRAINT "RuleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ReferencerRule derives from Rule
ALTER TABLE "SiteDirectory"."ReferencerRule" ADD CONSTRAINT "ReferencerRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ReferencerRule.ReferencingCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."ReferencerRule" ADD COLUMN "ReferencingCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ReferencerRule" ADD CONSTRAINT "ReferencerRule_FK_ReferencingCategory" FOREIGN KEY ("ReferencingCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ReferencedCategory is a collection property (many to many) of class ReferencerRule: [1..*]-[0..1]
CREATE TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" (
  "ReferencerRule" uuid NOT NULL,
  "ReferencedCategory" uuid NOT NULL,
  CONSTRAINT "ReferencerRule_ReferencedCategory_PK" PRIMARY KEY("ReferencerRule", "ReferencedCategory"),
  CONSTRAINT "ReferencerRule_ReferencedCategory_FK_Source" FOREIGN KEY ("ReferencerRule") REFERENCES "SiteDirectory"."ReferencerRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ReferencerRule_ReferencedCategory_FK_Target" FOREIGN KEY ("ReferencedCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferencerRule_ReferencedCategory_ValidFrom" ON "SiteDirectory"."ReferencerRule_ReferencedCategory" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRule_ReferencedCategory_ValidTo" ON "SiteDirectory"."ReferencerRule_ReferencedCategory" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" (LIKE "SiteDirectory"."ReferencerRule_ReferencedCategory");

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferencerRule_ReferencedCategoryAudit_ValidFrom" ON "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRule_ReferencedCategoryAudit_ValidTo" ON "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit" ("ValidTo");

CREATE TRIGGER ReferencerRule_ReferencedCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferencerRule_ReferencedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferencerRule_ReferencedCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferencerRule_ReferencedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER referencerrule_referencedcategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ReferencerRule_ReferencedCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ReferencerRule', 'SiteDirectory');
-- Class BinaryRelationshipRule derives from Rule
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- BinaryRelationshipRule.RelationshipCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD COLUMN "RelationshipCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRule_FK_RelationshipCategory" FOREIGN KEY ("RelationshipCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- BinaryRelationshipRule.SourceCategory is an association to Category: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD COLUMN "SourceCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRule_FK_SourceCategory" FOREIGN KEY ("SourceCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- BinaryRelationshipRule.TargetCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD COLUMN "TargetCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule" ADD CONSTRAINT "BinaryRelationshipRule_FK_TargetCategory" FOREIGN KEY ("TargetCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class MultiRelationshipRule derives from Rule
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" ADD CONSTRAINT "MultiRelationshipRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- MultiRelationshipRule.RelationshipCategory is an association to Category: [1..1]-[0..1]
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" ADD COLUMN "RelationshipCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."MultiRelationshipRule" ADD CONSTRAINT "MultiRelationshipRule_FK_RelationshipCategory" FOREIGN KEY ("RelationshipCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- RelatedCategory is a collection property (many to many) of class MultiRelationshipRule: [1..*]-[0..1]
CREATE TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" (
  "MultiRelationshipRule" uuid NOT NULL,
  "RelatedCategory" uuid NOT NULL,
  CONSTRAINT "MultiRelationshipRule_RelatedCategory_PK" PRIMARY KEY("MultiRelationshipRule", "RelatedCategory"),
  CONSTRAINT "MultiRelationshipRule_RelatedCategory_FK_Source" FOREIGN KEY ("MultiRelationshipRule") REFERENCES "SiteDirectory"."MultiRelationshipRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "MultiRelationshipRule_RelatedCategory_FK_Target" FOREIGN KEY ("RelatedCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategory_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategory_ValidTo" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory" ("ValidTo");

CREATE TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" (LIKE "SiteDirectory"."MultiRelationshipRule_RelatedCategory");

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategoryAudit_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRule_RelatedCategoryAudit_ValidTo" ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit" ("ValidTo");

CREATE TRIGGER MultiRelationshipRule_RelatedCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MultiRelationshipRule_RelatedCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER multirelationshiprule_relatedcategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('MultiRelationshipRule', 'SiteDirectory');
-- Class DecompositionRule derives from Rule
ALTER TABLE "SiteDirectory"."DecompositionRule" ADD CONSTRAINT "DecompositionRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- DecompositionRule.ContainingCategory is an association to Category: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."DecompositionRule" ADD COLUMN "ContainingCategory" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."DecompositionRule" ADD CONSTRAINT "DecompositionRule_FK_ContainingCategory" FOREIGN KEY ("ContainingCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ContainedCategory is a collection property (many to many) of class DecompositionRule: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" (
  "DecompositionRule" uuid NOT NULL,
  "ContainedCategory" uuid NOT NULL,
  CONSTRAINT "DecompositionRule_ContainedCategory_PK" PRIMARY KEY("DecompositionRule", "ContainedCategory"),
  CONSTRAINT "DecompositionRule_ContainedCategory_FK_Source" FOREIGN KEY ("DecompositionRule") REFERENCES "SiteDirectory"."DecompositionRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "DecompositionRule_ContainedCategory_FK_Target" FOREIGN KEY ("ContainedCategory") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DecompositionRule_ContainedCategory_ValidFrom" ON "SiteDirectory"."DecompositionRule_ContainedCategory" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRule_ContainedCategory_ValidTo" ON "SiteDirectory"."DecompositionRule_ContainedCategory" ("ValidTo");

CREATE TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" (LIKE "SiteDirectory"."DecompositionRule_ContainedCategory");

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DecompositionRule_ContainedCategoryAudit_ValidFrom" ON "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRule_ContainedCategoryAudit_ValidTo" ON "SiteDirectory"."DecompositionRule_ContainedCategory_Audit" ("ValidTo");

CREATE TRIGGER DecompositionRule_ContainedCategory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DecompositionRule_ContainedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DecompositionRule_ContainedCategory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DecompositionRule_ContainedCategory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER decompositionrule_containedcategory_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DecompositionRule_ContainedCategory"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('DecompositionRule', 'SiteDirectory');
-- Class ParameterizedCategoryRule derives from Rule
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" ADD CONSTRAINT "ParameterizedCategoryRuleDerivesFromRule" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Rule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ParameterizedCategoryRule.Category is an association to Category: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" ADD COLUMN "Category" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule" ADD CONSTRAINT "ParameterizedCategoryRule_FK_Category" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ParameterType is a collection property (many to many) of class ParameterizedCategoryRule: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" (
  "ParameterizedCategoryRule" uuid NOT NULL,
  "ParameterType" uuid NOT NULL,
  CONSTRAINT "ParameterizedCategoryRule_ParameterType_PK" PRIMARY KEY("ParameterizedCategoryRule", "ParameterType"),
  CONSTRAINT "ParameterizedCategoryRule_ParameterType_FK_Source" FOREIGN KEY ("ParameterizedCategoryRule") REFERENCES "SiteDirectory"."ParameterizedCategoryRule" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "ParameterizedCategoryRule_ParameterType_FK_Target" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterType_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterType_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" (LIKE "SiteDirectory"."ParameterizedCategoryRule_ParameterType");

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRule_ParameterTypeAudit_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit" ("ValidTo");

CREATE TRIGGER ParameterizedCategoryRule_ParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterizedCategoryRule_ParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER parameterizedcategoryrule_parametertype_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('ParameterizedCategoryRule', 'SiteDirectory');
-- Class Constant derives from DefinedThing
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "ConstantDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Constant.ParameterType is an association to ParameterType: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Constant" ADD COLUMN "ParameterType" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "Constant_FK_ParameterType" FOREIGN KEY ("ParameterType") REFERENCES "SiteDirectory"."ParameterType" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Constant.Scale is an optional association to MeasurementScale: [0..1]
ALTER TABLE "SiteDirectory"."Constant" ADD COLUMN "Scale" uuid;
ALTER TABLE "SiteDirectory"."Constant" ADD CONSTRAINT "Constant_FK_Scale" FOREIGN KEY ("Scale") REFERENCES "SiteDirectory"."MeasurementScale" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Category is a collection property (many to many) of class Constant: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."Constant_Category" (
  "Constant" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "Constant_Category_PK" PRIMARY KEY("Constant", "Category"),
  CONSTRAINT "Constant_Category_FK_Source" FOREIGN KEY ("Constant") REFERENCES "SiteDirectory"."Constant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Constant_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Constant_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Constant_Category_ValidFrom" ON "SiteDirectory"."Constant_Category" ("ValidFrom");
CREATE INDEX "Idx_Constant_Category_ValidTo" ON "SiteDirectory"."Constant_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."Constant_Category_Audit" (LIKE "SiteDirectory"."Constant_Category");

ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Constant_CategoryAudit_ValidFrom" ON "SiteDirectory"."Constant_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_Constant_CategoryAudit_ValidTo" ON "SiteDirectory"."Constant_Category_Audit" ("ValidTo");

CREATE TRIGGER Constant_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Constant_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Constant_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Constant_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER constant_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Constant_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Constant', 'SiteDirectory');
-- Class EngineeringModelSetup derives from DefinedThing
ALTER TABLE "SiteDirectory"."EngineeringModelSetup" ADD CONSTRAINT "EngineeringModelSetupDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Participant is contained (composite) by EngineeringModelSetup: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_Participant_Container" ON "SiteDirectory"."Participant" ("Container");
CREATE TRIGGER participant_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Participant"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- ActiveDomain is a collection property (many to many) of class EngineeringModelSetup: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" (
  "EngineeringModelSetup" uuid NOT NULL,
  "ActiveDomain" uuid NOT NULL,
  CONSTRAINT "EngineeringModelSetup_ActiveDomain_PK" PRIMARY KEY("EngineeringModelSetup", "ActiveDomain"),
  CONSTRAINT "EngineeringModelSetup_ActiveDomain_FK_Source" FOREIGN KEY ("EngineeringModelSetup") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "EngineeringModelSetup_ActiveDomain_FK_Target" FOREIGN KEY ("ActiveDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomain_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomain_ValidTo" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain" ("ValidTo");

CREATE TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" (LIKE "SiteDirectory"."EngineeringModelSetup_ActiveDomain");

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomainAudit_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetup_ActiveDomainAudit_ValidTo" ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit" ("ValidTo");

CREATE TRIGGER EngineeringModelSetup_ActiveDomain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelSetup_ActiveDomain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER engineeringmodelsetup_activedomain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('EngineeringModelSetup', 'SiteDirectory');
-- ModelReferenceDataLibrary is contained (composite) by EngineeringModelSetup: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" ADD CONSTRAINT "ModelReferenceDataLibrary_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_ModelReferenceDataLibrary_Container" ON "SiteDirectory"."ModelReferenceDataLibrary" ("Container");
CREATE TRIGGER modelreferencedatalibrary_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."ModelReferenceDataLibrary"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- IterationSetup is contained (composite) by EngineeringModelSetup: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."IterationSetup" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."IterationSetup" ADD CONSTRAINT "IterationSetup_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."EngineeringModelSetup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_IterationSetup_Container" ON "SiteDirectory"."IterationSetup" ("Container");
CREATE TRIGGER iterationsetup_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."IterationSetup"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class Participant derives from Thing
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "ParticipantDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Participant.Person is an association to Person: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "Person" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_Person" FOREIGN KEY ("Person") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Participant.Role is an association to ParticipantRole: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "Role" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_Role" FOREIGN KEY ("Role") REFERENCES "SiteDirectory"."ParticipantRole" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Domain is a collection property (many to many) of class Participant: [1..*]-[0..*]
CREATE TABLE "SiteDirectory"."Participant_Domain" (
  "Participant" uuid NOT NULL,
  "Domain" uuid NOT NULL,
  CONSTRAINT "Participant_Domain_PK" PRIMARY KEY("Participant", "Domain"),
  CONSTRAINT "Participant_Domain_FK_Source" FOREIGN KEY ("Participant") REFERENCES "SiteDirectory"."Participant" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "Participant_Domain_FK_Target" FOREIGN KEY ("Domain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Domain" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."Participant_Domain"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Participant_Domain_ValidFrom" ON "SiteDirectory"."Participant_Domain" ("ValidFrom");
CREATE INDEX "Idx_Participant_Domain_ValidTo" ON "SiteDirectory"."Participant_Domain" ("ValidTo");

CREATE TABLE "SiteDirectory"."Participant_Domain_Audit" (LIKE "SiteDirectory"."Participant_Domain");

ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Domain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_Participant_DomainAudit_ValidFrom" ON "SiteDirectory"."Participant_Domain_Audit" ("ValidFrom");
CREATE INDEX "Idx_Participant_DomainAudit_ValidTo" ON "SiteDirectory"."Participant_Domain_Audit" ("ValidTo");

CREATE TRIGGER Participant_Domain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Participant_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Participant_Domain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Participant_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER participant_domain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."Participant_Domain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Participant', 'SiteDirectory');
-- Participant.SelectedDomain is an association to DomainOfExpertise: [1..1]-[0..*]
ALTER TABLE "SiteDirectory"."Participant" ADD COLUMN "SelectedDomain" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."Participant" ADD CONSTRAINT "Participant_FK_SelectedDomain" FOREIGN KEY ("SelectedDomain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class ModelReferenceDataLibrary derives from ReferenceDataLibrary
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary" ADD CONSTRAINT "ModelReferenceDataLibraryDerivesFromReferenceDataLibrary" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ReferenceDataLibrary" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class IterationSetup derives from Thing
ALTER TABLE "SiteDirectory"."IterationSetup" ADD CONSTRAINT "IterationSetupDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- IterationSetup.SourceIterationSetup is an optional association to IterationSetup: [0..1]
ALTER TABLE "SiteDirectory"."IterationSetup" ADD COLUMN "SourceIterationSetup" uuid;
ALTER TABLE "SiteDirectory"."IterationSetup" ADD CONSTRAINT "IterationSetup_FK_SourceIterationSetup" FOREIGN KEY ("SourceIterationSetup") REFERENCES "SiteDirectory"."IterationSetup" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Class PersonRole derives from DefinedThing
ALTER TABLE "SiteDirectory"."PersonRole" ADD CONSTRAINT "PersonRoleDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- PersonPermission is contained (composite) by PersonRole: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."PersonPermission" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."PersonPermission" ADD CONSTRAINT "PersonPermission_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."PersonRole" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_PersonPermission_Container" ON "SiteDirectory"."PersonPermission" ("Container");
CREATE TRIGGER personpermission_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."PersonPermission"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class PersonPermission derives from Thing
ALTER TABLE "SiteDirectory"."PersonPermission" ADD CONSTRAINT "PersonPermissionDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class SiteLogEntry derives from Thing
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD CONSTRAINT "SiteLogEntryDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Category is a collection property (many to many) of class SiteLogEntry: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_Category" (
  "SiteLogEntry" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "SiteLogEntry_Category_PK" PRIMARY KEY("SiteLogEntry", "Category"),
  CONSTRAINT "SiteLogEntry_Category_FK_Source" FOREIGN KEY ("SiteLogEntry") REFERENCES "SiteDirectory"."SiteLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "SiteLogEntry_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteLogEntry_Category_ValidFrom" ON "SiteDirectory"."SiteLogEntry_Category" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_Category_ValidTo" ON "SiteDirectory"."SiteLogEntry_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" (LIKE "SiteDirectory"."SiteLogEntry_Category");

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntry_CategoryAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_CategoryAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_Category_Audit" ("ValidTo");

CREATE TRIGGER SiteLogEntry_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER sitelogentry_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('SiteLogEntry', 'SiteDirectory');
-- SiteLogEntry.Author is an optional association to Person: [0..1]-[1..1]
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD COLUMN "Author" uuid;
ALTER TABLE "SiteDirectory"."SiteLogEntry" ADD CONSTRAINT "SiteLogEntry_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- AffectedItemIid is a collection property of class SiteLogEntry: [0..*]
CREATE TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" (
  "SiteLogEntry" uuid NOT NULL,
  "AffectedItemIid" uuid NOT NULL,
  CONSTRAINT "SiteLogEntry_AffectedItemIid_PK" PRIMARY KEY("SiteLogEntry","AffectedItemIid"),
  CONSTRAINT "SiteLogEntry_AffectedItemIid_FK_Source" FOREIGN KEY ("SiteLogEntry") REFERENCES "SiteDirectory"."SiteLogEntry" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE
);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid" SET (autovacuum_analyze_threshold = 2500);  
ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIid_ValidFrom" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIid_ValidTo" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" (LIKE "SiteDirectory"."SiteLogEntry_AffectedItemIid");

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIidAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_AffectedItemIidAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit" ("ValidTo");

CREATE TRIGGER SiteLogEntry_AffectedItemIid_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_AffectedItemIid_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER sitelogentry_affecteditemiid_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteLogEntry_AffectedItemIid"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('SiteLogEntry', 'SiteDirectory');
-- Class DomainOfExpertiseGroup derives from DefinedThing
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup" ADD CONSTRAINT "DomainOfExpertiseGroupDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Domain is a collection property (many to many) of class DomainOfExpertiseGroup: [0..*]-[1..*]
CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" (
  "DomainOfExpertiseGroup" uuid NOT NULL,
  "Domain" uuid NOT NULL,
  CONSTRAINT "DomainOfExpertiseGroup_Domain_PK" PRIMARY KEY("DomainOfExpertiseGroup", "Domain"),
  CONSTRAINT "DomainOfExpertiseGroup_Domain_FK_Source" FOREIGN KEY ("DomainOfExpertiseGroup") REFERENCES "SiteDirectory"."DomainOfExpertiseGroup" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "DomainOfExpertiseGroup_Domain_FK_Target" FOREIGN KEY ("Domain") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DomainOfExpertiseGroup_Domain_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroup_Domain_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain" ("ValidTo");

CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" (LIKE "SiteDirectory"."DomainOfExpertiseGroup_Domain");

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertiseGroup_DomainAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroup_DomainAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit" ("ValidTo");

CREATE TRIGGER DomainOfExpertiseGroup_Domain_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertiseGroup_Domain_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER domainofexpertisegroup_domain_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertiseGroup_Domain"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('DomainOfExpertiseGroup', 'SiteDirectory');
-- Class DomainOfExpertise derives from DefinedThing
ALTER TABLE "SiteDirectory"."DomainOfExpertise" ADD CONSTRAINT "DomainOfExpertiseDerivesFromDefinedThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DefinedThing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Category is a collection property (many to many) of class DomainOfExpertise: [0..*]-[0..*]
CREATE TABLE "SiteDirectory"."DomainOfExpertise_Category" (
  "DomainOfExpertise" uuid NOT NULL,
  "Category" uuid NOT NULL,
  CONSTRAINT "DomainOfExpertise_Category_PK" PRIMARY KEY("DomainOfExpertise", "Category"),
  CONSTRAINT "DomainOfExpertise_Category_FK_Source" FOREIGN KEY ("DomainOfExpertise") REFERENCES "SiteDirectory"."DomainOfExpertise" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE,
  CONSTRAINT "DomainOfExpertise_Category_FK_Target" FOREIGN KEY ("Category") REFERENCES "SiteDirectory"."Category" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE
);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category" SET (autovacuum_analyze_threshold = 2500);  

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DomainOfExpertise_Category_ValidFrom" ON "SiteDirectory"."DomainOfExpertise_Category" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertise_Category_ValidTo" ON "SiteDirectory"."DomainOfExpertise_Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" (LIKE "SiteDirectory"."DomainOfExpertise_Category");

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertise_CategoryAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertise_Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertise_CategoryAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertise_Category_Audit" ("ValidTo");

CREATE TRIGGER DomainOfExpertise_Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertise_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertise_Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertise_Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE TRIGGER domainofexpertise_category_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."DomainOfExpertise_Category"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('DomainOfExpertise', 'SiteDirectory');
-- Class NaturalLanguage derives from Thing
ALTER TABLE "SiteDirectory"."NaturalLanguage" ADD CONSTRAINT "NaturalLanguageDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class GenericAnnotation derives from Thing
ALTER TABLE "SiteDirectory"."GenericAnnotation" ADD CONSTRAINT "GenericAnnotationDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class SiteDirectoryDataAnnotation derives from GenericAnnotation
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotationDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- SiteDirectoryThingReference is contained (composite) by SiteDirectoryDataAnnotation: [1..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" ADD CONSTRAINT "SiteDirectoryThingReference_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectoryDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_SiteDirectoryThingReference_Container" ON "SiteDirectory"."SiteDirectoryThingReference" ("Container");
CREATE TRIGGER sitedirectorythingreference_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteDirectoryThingReference"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- SiteDirectoryDataAnnotation.Author is an association to Person: [1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotation_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- SiteDirectoryDataAnnotation.PrimaryAnnotatedThing is an association to SiteDirectoryThingReference: [1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD COLUMN "PrimaryAnnotatedThing" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation" ADD CONSTRAINT "SiteDirectoryDataAnnotation_FK_PrimaryAnnotatedThing" FOREIGN KEY ("PrimaryAnnotatedThing") REFERENCES "SiteDirectory"."SiteDirectoryThingReference" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- SiteDirectoryDataDiscussionItem is contained (composite) by SiteDirectoryDataAnnotation: [0..*]-[1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD COLUMN "Container" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD CONSTRAINT "SiteDirectoryDataDiscussionItem_FK_Container" FOREIGN KEY ("Container") REFERENCES "SiteDirectory"."SiteDirectoryDataAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- add index on container
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItem_Container" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem" ("Container");
CREATE TRIGGER sitedirectorydatadiscussionitem_apply_revision
  BEFORE INSERT OR UPDATE OR DELETE 
  ON "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".revision_management('Container', 'SiteDirectory', 'SiteDirectory');
-- Class ThingReference derives from Thing
ALTER TABLE "SiteDirectory"."ThingReference" ADD CONSTRAINT "ThingReferenceDerivesFromThing" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."Thing" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- ThingReference.ReferencedThing is an association to Thing: [1..1]-[1..1]
ALTER TABLE "SiteDirectory"."ThingReference" ADD COLUMN "ReferencedThing" uuid NOT NULL;
-- Class SiteDirectoryThingReference derives from ThingReference
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference" ADD CONSTRAINT "SiteDirectoryThingReferenceDerivesFromThingReference" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."ThingReference" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- Class DiscussionItem derives from GenericAnnotation
ALTER TABLE "SiteDirectory"."DiscussionItem" ADD CONSTRAINT "DiscussionItemDerivesFromGenericAnnotation" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."GenericAnnotation" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- DiscussionItem.ReplyTo is an optional association to DiscussionItem: [0..1]
ALTER TABLE "SiteDirectory"."DiscussionItem" ADD COLUMN "ReplyTo" uuid;
ALTER TABLE "SiteDirectory"."DiscussionItem" ADD CONSTRAINT "DiscussionItem_FK_ReplyTo" FOREIGN KEY ("ReplyTo") REFERENCES "SiteDirectory"."DiscussionItem" ("Iid") ON UPDATE CASCADE ON DELETE SET NULL DEFERRABLE;
-- Class SiteDirectoryDataDiscussionItem derives from DiscussionItem
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD CONSTRAINT "SiteDirectoryDataDiscussionItemDerivesFromDiscussionItem" FOREIGN KEY ("Iid") REFERENCES "SiteDirectory"."DiscussionItem" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
-- SiteDirectoryDataDiscussionItem.Author is an association to Person: [1..1]
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD COLUMN "Author" uuid NOT NULL;
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem" ADD CONSTRAINT "SiteDirectoryDataDiscussionItem_FK_Author" FOREIGN KEY ("Author") REFERENCES "SiteDirectory"."Person" ("Iid") ON UPDATE CASCADE ON DELETE CASCADE DEFERRABLE;
ALTER TABLE "SiteDirectory"."Thing"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Thing_ValidFrom" ON "SiteDirectory"."Thing" ("ValidFrom");
CREATE INDEX "Idx_Thing_ValidTo" ON "SiteDirectory"."Thing" ("ValidTo");

CREATE TABLE "SiteDirectory"."Thing_Audit" (LIKE "SiteDirectory"."Thing");

ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Thing_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Thing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingAudit_ValidFrom" ON "SiteDirectory"."Thing_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingAudit_ValidTo" ON "SiteDirectory"."Thing_Audit" ("ValidTo");

CREATE TRIGGER Thing_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Thing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Thing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."TopContainer"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_TopContainer_ValidFrom" ON "SiteDirectory"."TopContainer" ("ValidFrom");
CREATE INDEX "Idx_TopContainer_ValidTo" ON "SiteDirectory"."TopContainer" ("ValidTo");

CREATE TABLE "SiteDirectory"."TopContainer_Audit" (LIKE "SiteDirectory"."TopContainer");

ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TopContainer_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."TopContainer_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TopContainerAudit_ValidFrom" ON "SiteDirectory"."TopContainer_Audit" ("ValidFrom");
CREATE INDEX "Idx_TopContainerAudit_ValidTo" ON "SiteDirectory"."TopContainer_Audit" ("ValidTo");

CREATE TRIGGER TopContainer_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TopContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TopContainer_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TopContainer"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SiteDirectory"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteDirectory_ValidFrom" ON "SiteDirectory"."SiteDirectory" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectory_ValidTo" ON "SiteDirectory"."SiteDirectory" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteDirectory_Audit" (LIKE "SiteDirectory"."SiteDirectory");

ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectory_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryAudit_ValidFrom" ON "SiteDirectory"."SiteDirectory_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryAudit_ValidTo" ON "SiteDirectory"."SiteDirectory_Audit" ("ValidTo");

CREATE TRIGGER SiteDirectory_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectory_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectory"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Organization"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Organization_ValidFrom" ON "SiteDirectory"."Organization" ("ValidFrom");
CREATE INDEX "Idx_Organization_ValidTo" ON "SiteDirectory"."Organization" ("ValidTo");

CREATE TABLE "SiteDirectory"."Organization_Audit" (LIKE "SiteDirectory"."Organization");

ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Organization_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Organization_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrganizationAudit_ValidFrom" ON "SiteDirectory"."Organization_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrganizationAudit_ValidTo" ON "SiteDirectory"."Organization_Audit" ("ValidTo");

CREATE TRIGGER Organization_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Organization"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Organization_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Organization"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Person"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Person_ValidFrom" ON "SiteDirectory"."Person" ("ValidFrom");
CREATE INDEX "Idx_Person_ValidTo" ON "SiteDirectory"."Person" ("ValidTo");

CREATE TABLE "SiteDirectory"."Person_Audit" (LIKE "SiteDirectory"."Person");

ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Person_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Person_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PersonAudit_ValidFrom" ON "SiteDirectory"."Person_Audit" ("ValidFrom");
CREATE INDEX "Idx_PersonAudit_ValidTo" ON "SiteDirectory"."Person_Audit" ("ValidTo");

CREATE TRIGGER Person_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Person"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Person_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Person"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."EmailAddress"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_EmailAddress_ValidFrom" ON "SiteDirectory"."EmailAddress" ("ValidFrom");
CREATE INDEX "Idx_EmailAddress_ValidTo" ON "SiteDirectory"."EmailAddress" ("ValidTo");

CREATE TABLE "SiteDirectory"."EmailAddress_Audit" (LIKE "SiteDirectory"."EmailAddress");

ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EmailAddress_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."EmailAddress_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EmailAddressAudit_ValidFrom" ON "SiteDirectory"."EmailAddress_Audit" ("ValidFrom");
CREATE INDEX "Idx_EmailAddressAudit_ValidTo" ON "SiteDirectory"."EmailAddress_Audit" ("ValidTo");

CREATE TRIGGER EmailAddress_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EmailAddress"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EmailAddress_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EmailAddress"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."TelephoneNumber"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_TelephoneNumber_ValidFrom" ON "SiteDirectory"."TelephoneNumber" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumber_ValidTo" ON "SiteDirectory"."TelephoneNumber" ("ValidTo");

CREATE TABLE "SiteDirectory"."TelephoneNumber_Audit" (LIKE "SiteDirectory"."TelephoneNumber");

ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."TelephoneNumber_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TelephoneNumberAudit_ValidFrom" ON "SiteDirectory"."TelephoneNumber_Audit" ("ValidFrom");
CREATE INDEX "Idx_TelephoneNumberAudit_ValidTo" ON "SiteDirectory"."TelephoneNumber_Audit" ("ValidTo");

CREATE TRIGGER TelephoneNumber_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TelephoneNumber"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TelephoneNumber_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TelephoneNumber"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."UserPreference"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_UserPreference_ValidFrom" ON "SiteDirectory"."UserPreference" ("ValidFrom");
CREATE INDEX "Idx_UserPreference_ValidTo" ON "SiteDirectory"."UserPreference" ("ValidTo");

CREATE TABLE "SiteDirectory"."UserPreference_Audit" (LIKE "SiteDirectory"."UserPreference");

ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UserPreference_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."UserPreference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UserPreferenceAudit_ValidFrom" ON "SiteDirectory"."UserPreference_Audit" ("ValidFrom");
CREATE INDEX "Idx_UserPreferenceAudit_ValidTo" ON "SiteDirectory"."UserPreference_Audit" ("ValidTo");

CREATE TRIGGER UserPreference_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."UserPreference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UserPreference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."UserPreference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DefinedThing"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DefinedThing_ValidFrom" ON "SiteDirectory"."DefinedThing" ("ValidFrom");
CREATE INDEX "Idx_DefinedThing_ValidTo" ON "SiteDirectory"."DefinedThing" ("ValidTo");

CREATE TABLE "SiteDirectory"."DefinedThing_Audit" (LIKE "SiteDirectory"."DefinedThing");

ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DefinedThing_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DefinedThing_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DefinedThingAudit_ValidFrom" ON "SiteDirectory"."DefinedThing_Audit" ("ValidFrom");
CREATE INDEX "Idx_DefinedThingAudit_ValidTo" ON "SiteDirectory"."DefinedThing_Audit" ("ValidTo");

CREATE TRIGGER DefinedThing_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DefinedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DefinedThing_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DefinedThing"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ParticipantRole"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParticipantRole_ValidFrom" ON "SiteDirectory"."ParticipantRole" ("ValidFrom");
CREATE INDEX "Idx_ParticipantRole_ValidTo" ON "SiteDirectory"."ParticipantRole" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParticipantRole_Audit" (LIKE "SiteDirectory"."ParticipantRole");

ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantRole_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParticipantRoleAudit_ValidFrom" ON "SiteDirectory"."ParticipantRole_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParticipantRoleAudit_ValidTo" ON "SiteDirectory"."ParticipantRole_Audit" ("ValidTo");

CREATE TRIGGER ParticipantRole_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParticipantRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParticipantRole_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParticipantRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Alias"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Alias_ValidFrom" ON "SiteDirectory"."Alias" ("ValidFrom");
CREATE INDEX "Idx_Alias_ValidTo" ON "SiteDirectory"."Alias" ("ValidTo");

CREATE TABLE "SiteDirectory"."Alias_Audit" (LIKE "SiteDirectory"."Alias");

ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Alias_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Alias_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_AliasAudit_ValidFrom" ON "SiteDirectory"."Alias_Audit" ("ValidFrom");
CREATE INDEX "Idx_AliasAudit_ValidTo" ON "SiteDirectory"."Alias_Audit" ("ValidTo");

CREATE TRIGGER Alias_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Alias"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Alias_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Alias"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Definition"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Definition_ValidFrom" ON "SiteDirectory"."Definition" ("ValidFrom");
CREATE INDEX "Idx_Definition_ValidTo" ON "SiteDirectory"."Definition" ("ValidTo");

CREATE TABLE "SiteDirectory"."Definition_Audit" (LIKE "SiteDirectory"."Definition");

ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Definition_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Definition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DefinitionAudit_ValidFrom" ON "SiteDirectory"."Definition_Audit" ("ValidFrom");
CREATE INDEX "Idx_DefinitionAudit_ValidTo" ON "SiteDirectory"."Definition_Audit" ("ValidTo");

CREATE TRIGGER Definition_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Definition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Definition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Definition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Citation"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Citation_ValidFrom" ON "SiteDirectory"."Citation" ("ValidFrom");
CREATE INDEX "Idx_Citation_ValidTo" ON "SiteDirectory"."Citation" ("ValidTo");

CREATE TABLE "SiteDirectory"."Citation_Audit" (LIKE "SiteDirectory"."Citation");

ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Citation_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Citation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CitationAudit_ValidFrom" ON "SiteDirectory"."Citation_Audit" ("ValidFrom");
CREATE INDEX "Idx_CitationAudit_ValidTo" ON "SiteDirectory"."Citation_Audit" ("ValidTo");

CREATE TRIGGER Citation_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Citation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Citation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Citation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."HyperLink"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_HyperLink_ValidFrom" ON "SiteDirectory"."HyperLink" ("ValidFrom");
CREATE INDEX "Idx_HyperLink_ValidTo" ON "SiteDirectory"."HyperLink" ("ValidTo");

CREATE TABLE "SiteDirectory"."HyperLink_Audit" (LIKE "SiteDirectory"."HyperLink");

ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."HyperLink_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."HyperLink_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_HyperLinkAudit_ValidFrom" ON "SiteDirectory"."HyperLink_Audit" ("ValidFrom");
CREATE INDEX "Idx_HyperLinkAudit_ValidTo" ON "SiteDirectory"."HyperLink_Audit" ("ValidTo");

CREATE TRIGGER HyperLink_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."HyperLink"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER HyperLink_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."HyperLink"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ParticipantPermission"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParticipantPermission_ValidFrom" ON "SiteDirectory"."ParticipantPermission" ("ValidFrom");
CREATE INDEX "Idx_ParticipantPermission_ValidTo" ON "SiteDirectory"."ParticipantPermission" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParticipantPermission_Audit" (LIKE "SiteDirectory"."ParticipantPermission");

ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParticipantPermission_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParticipantPermissionAudit_ValidFrom" ON "SiteDirectory"."ParticipantPermission_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParticipantPermissionAudit_ValidTo" ON "SiteDirectory"."ParticipantPermission_Audit" ("ValidTo");

CREATE TRIGGER ParticipantPermission_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParticipantPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParticipantPermission_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParticipantPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ReferenceDataLibrary"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferenceDataLibrary_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibrary_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" (LIKE "SiteDirectory"."ReferenceDataLibrary");

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceDataLibrary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceDataLibraryAudit_ValidFrom" ON "SiteDirectory"."ReferenceDataLibrary_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceDataLibraryAudit_ValidTo" ON "SiteDirectory"."ReferenceDataLibrary_Audit" ("ValidTo");

CREATE TRIGGER ReferenceDataLibrary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceDataLibrary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteReferenceDataLibrary_ValidFrom" ON "SiteDirectory"."SiteReferenceDataLibrary" ("ValidFrom");
CREATE INDEX "Idx_SiteReferenceDataLibrary_ValidTo" ON "SiteDirectory"."SiteReferenceDataLibrary" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" (LIKE "SiteDirectory"."SiteReferenceDataLibrary");

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteReferenceDataLibrary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteReferenceDataLibraryAudit_ValidFrom" ON "SiteDirectory"."SiteReferenceDataLibrary_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteReferenceDataLibraryAudit_ValidTo" ON "SiteDirectory"."SiteReferenceDataLibrary_Audit" ("ValidTo");

CREATE TRIGGER SiteReferenceDataLibrary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteReferenceDataLibrary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Category"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Category_ValidFrom" ON "SiteDirectory"."Category" ("ValidFrom");
CREATE INDEX "Idx_Category_ValidTo" ON "SiteDirectory"."Category" ("ValidTo");

CREATE TABLE "SiteDirectory"."Category_Audit" (LIKE "SiteDirectory"."Category");

ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Category_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Category_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CategoryAudit_ValidFrom" ON "SiteDirectory"."Category_Audit" ("ValidFrom");
CREATE INDEX "Idx_CategoryAudit_ValidTo" ON "SiteDirectory"."Category_Audit" ("ValidTo");

CREATE TRIGGER Category_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Category_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Category"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParameterType_ValidFrom" ON "SiteDirectory"."ParameterType" ("ValidFrom");
CREATE INDEX "Idx_ParameterType_ValidTo" ON "SiteDirectory"."ParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParameterType_Audit" (LIKE "SiteDirectory"."ParameterType");

ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterTypeAudit_ValidTo" ON "SiteDirectory"."ParameterType_Audit" ("ValidTo");

CREATE TRIGGER ParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."CompoundParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_CompoundParameterType_ValidFrom" ON "SiteDirectory"."CompoundParameterType" ("ValidFrom");
CREATE INDEX "Idx_CompoundParameterType_ValidTo" ON "SiteDirectory"."CompoundParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."CompoundParameterType_Audit" (LIKE "SiteDirectory"."CompoundParameterType");

ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."CompoundParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CompoundParameterTypeAudit_ValidFrom" ON "SiteDirectory"."CompoundParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_CompoundParameterTypeAudit_ValidTo" ON "SiteDirectory"."CompoundParameterType_Audit" ("ValidTo");

CREATE TRIGGER CompoundParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."CompoundParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER CompoundParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."CompoundParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ArrayParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ArrayParameterType_ValidFrom" ON "SiteDirectory"."ArrayParameterType" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterType_ValidTo" ON "SiteDirectory"."ArrayParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."ArrayParameterType_Audit" (LIKE "SiteDirectory"."ArrayParameterType");

ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ArrayParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ArrayParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ArrayParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ArrayParameterTypeAudit_ValidTo" ON "SiteDirectory"."ArrayParameterType_Audit" ("ValidTo");

CREATE TRIGGER ArrayParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ArrayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ArrayParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ArrayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ParameterTypeComponent"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParameterTypeComponent_ValidFrom" ON "SiteDirectory"."ParameterTypeComponent" ("ValidFrom");
CREATE INDEX "Idx_ParameterTypeComponent_ValidTo" ON "SiteDirectory"."ParameterTypeComponent" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParameterTypeComponent_Audit" (LIKE "SiteDirectory"."ParameterTypeComponent");

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterTypeComponent_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterTypeComponentAudit_ValidFrom" ON "SiteDirectory"."ParameterTypeComponent_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterTypeComponentAudit_ValidTo" ON "SiteDirectory"."ParameterTypeComponent_Audit" ("ValidTo");

CREATE TRIGGER ParameterTypeComponent_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterTypeComponent"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterTypeComponent_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterTypeComponent"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ScalarParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ScalarParameterType_ValidFrom" ON "SiteDirectory"."ScalarParameterType" ("ValidFrom");
CREATE INDEX "Idx_ScalarParameterType_ValidTo" ON "SiteDirectory"."ScalarParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."ScalarParameterType_Audit" (LIKE "SiteDirectory"."ScalarParameterType");

ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScalarParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ScalarParameterTypeAudit_ValidFrom" ON "SiteDirectory"."ScalarParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_ScalarParameterTypeAudit_ValidTo" ON "SiteDirectory"."ScalarParameterType_Audit" ("ValidTo");

CREATE TRIGGER ScalarParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ScalarParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ScalarParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ScalarParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."EnumerationParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_EnumerationParameterType_ValidFrom" ON "SiteDirectory"."EnumerationParameterType" ("ValidFrom");
CREATE INDEX "Idx_EnumerationParameterType_ValidTo" ON "SiteDirectory"."EnumerationParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."EnumerationParameterType_Audit" (LIKE "SiteDirectory"."EnumerationParameterType");

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EnumerationParameterTypeAudit_ValidFrom" ON "SiteDirectory"."EnumerationParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_EnumerationParameterTypeAudit_ValidTo" ON "SiteDirectory"."EnumerationParameterType_Audit" ("ValidTo");

CREATE TRIGGER EnumerationParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EnumerationParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EnumerationParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EnumerationParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."EnumerationValueDefinition"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_EnumerationValueDefinition_ValidFrom" ON "SiteDirectory"."EnumerationValueDefinition" ("ValidFrom");
CREATE INDEX "Idx_EnumerationValueDefinition_ValidTo" ON "SiteDirectory"."EnumerationValueDefinition" ("ValidTo");

CREATE TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" (LIKE "SiteDirectory"."EnumerationValueDefinition");

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."EnumerationValueDefinition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EnumerationValueDefinitionAudit_ValidFrom" ON "SiteDirectory"."EnumerationValueDefinition_Audit" ("ValidFrom");
CREATE INDEX "Idx_EnumerationValueDefinitionAudit_ValidTo" ON "SiteDirectory"."EnumerationValueDefinition_Audit" ("ValidTo");

CREATE TRIGGER EnumerationValueDefinition_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EnumerationValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EnumerationValueDefinition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EnumerationValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."BooleanParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_BooleanParameterType_ValidFrom" ON "SiteDirectory"."BooleanParameterType" ("ValidFrom");
CREATE INDEX "Idx_BooleanParameterType_ValidTo" ON "SiteDirectory"."BooleanParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."BooleanParameterType_Audit" (LIKE "SiteDirectory"."BooleanParameterType");

ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."BooleanParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BooleanParameterTypeAudit_ValidFrom" ON "SiteDirectory"."BooleanParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_BooleanParameterTypeAudit_ValidTo" ON "SiteDirectory"."BooleanParameterType_Audit" ("ValidTo");

CREATE TRIGGER BooleanParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."BooleanParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BooleanParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."BooleanParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DateParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DateParameterType_ValidFrom" ON "SiteDirectory"."DateParameterType" ("ValidFrom");
CREATE INDEX "Idx_DateParameterType_ValidTo" ON "SiteDirectory"."DateParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."DateParameterType_Audit" (LIKE "SiteDirectory"."DateParameterType");

ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DateParameterTypeAudit_ValidFrom" ON "SiteDirectory"."DateParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_DateParameterTypeAudit_ValidTo" ON "SiteDirectory"."DateParameterType_Audit" ("ValidTo");

CREATE TRIGGER DateParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DateParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DateParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DateParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."TextParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_TextParameterType_ValidFrom" ON "SiteDirectory"."TextParameterType" ("ValidFrom");
CREATE INDEX "Idx_TextParameterType_ValidTo" ON "SiteDirectory"."TextParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."TextParameterType_Audit" (LIKE "SiteDirectory"."TextParameterType");

ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TextParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."TextParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TextParameterTypeAudit_ValidFrom" ON "SiteDirectory"."TextParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_TextParameterTypeAudit_ValidTo" ON "SiteDirectory"."TextParameterType_Audit" ("ValidTo");

CREATE TRIGGER TextParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TextParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TextParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TextParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DateTimeParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DateTimeParameterType_ValidFrom" ON "SiteDirectory"."DateTimeParameterType" ("ValidFrom");
CREATE INDEX "Idx_DateTimeParameterType_ValidTo" ON "SiteDirectory"."DateTimeParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."DateTimeParameterType_Audit" (LIKE "SiteDirectory"."DateTimeParameterType");

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DateTimeParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DateTimeParameterTypeAudit_ValidFrom" ON "SiteDirectory"."DateTimeParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_DateTimeParameterTypeAudit_ValidTo" ON "SiteDirectory"."DateTimeParameterType_Audit" ("ValidTo");

CREATE TRIGGER DateTimeParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DateTimeParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DateTimeParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DateTimeParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."TimeOfDayParameterType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_TimeOfDayParameterType_ValidFrom" ON "SiteDirectory"."TimeOfDayParameterType" ("ValidFrom");
CREATE INDEX "Idx_TimeOfDayParameterType_ValidTo" ON "SiteDirectory"."TimeOfDayParameterType" ("ValidTo");

CREATE TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" (LIKE "SiteDirectory"."TimeOfDayParameterType");

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."TimeOfDayParameterType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TimeOfDayParameterTypeAudit_ValidFrom" ON "SiteDirectory"."TimeOfDayParameterType_Audit" ("ValidFrom");
CREATE INDEX "Idx_TimeOfDayParameterTypeAudit_ValidTo" ON "SiteDirectory"."TimeOfDayParameterType_Audit" ("ValidTo");

CREATE TRIGGER TimeOfDayParameterType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."TimeOfDayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER TimeOfDayParameterType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."TimeOfDayParameterType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."QuantityKind"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_QuantityKind_ValidFrom" ON "SiteDirectory"."QuantityKind" ("ValidFrom");
CREATE INDEX "Idx_QuantityKind_ValidTo" ON "SiteDirectory"."QuantityKind" ("ValidTo");

CREATE TABLE "SiteDirectory"."QuantityKind_Audit" (LIKE "SiteDirectory"."QuantityKind");

ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_QuantityKindAudit_ValidFrom" ON "SiteDirectory"."QuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_QuantityKindAudit_ValidTo" ON "SiteDirectory"."QuantityKind_Audit" ("ValidTo");

CREATE TRIGGER QuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."QuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER QuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."QuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SpecializedQuantityKind"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SpecializedQuantityKind_ValidFrom" ON "SiteDirectory"."SpecializedQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_SpecializedQuantityKind_ValidTo" ON "SiteDirectory"."SpecializedQuantityKind" ("ValidTo");

CREATE TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" (LIKE "SiteDirectory"."SpecializedQuantityKind");

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SpecializedQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SpecializedQuantityKindAudit_ValidFrom" ON "SiteDirectory"."SpecializedQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_SpecializedQuantityKindAudit_ValidTo" ON "SiteDirectory"."SpecializedQuantityKind_Audit" ("ValidTo");

CREATE TRIGGER SpecializedQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SpecializedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SpecializedQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SpecializedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SimpleQuantityKind"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SimpleQuantityKind_ValidFrom" ON "SiteDirectory"."SimpleQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_SimpleQuantityKind_ValidTo" ON "SiteDirectory"."SimpleQuantityKind" ("ValidTo");

CREATE TABLE "SiteDirectory"."SimpleQuantityKind_Audit" (LIKE "SiteDirectory"."SimpleQuantityKind");

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SimpleQuantityKindAudit_ValidFrom" ON "SiteDirectory"."SimpleQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_SimpleQuantityKindAudit_ValidTo" ON "SiteDirectory"."SimpleQuantityKind_Audit" ("ValidTo");

CREATE TRIGGER SimpleQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SimpleQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SimpleQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SimpleQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DerivedQuantityKind"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DerivedQuantityKind_ValidFrom" ON "SiteDirectory"."DerivedQuantityKind" ("ValidFrom");
CREATE INDEX "Idx_DerivedQuantityKind_ValidTo" ON "SiteDirectory"."DerivedQuantityKind" ("ValidTo");

CREATE TABLE "SiteDirectory"."DerivedQuantityKind_Audit" (LIKE "SiteDirectory"."DerivedQuantityKind");

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedQuantityKind_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DerivedQuantityKindAudit_ValidFrom" ON "SiteDirectory"."DerivedQuantityKind_Audit" ("ValidFrom");
CREATE INDEX "Idx_DerivedQuantityKindAudit_ValidTo" ON "SiteDirectory"."DerivedQuantityKind_Audit" ("ValidTo");

CREATE TRIGGER DerivedQuantityKind_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DerivedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DerivedQuantityKind_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DerivedQuantityKind"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."QuantityKindFactor"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_QuantityKindFactor_ValidFrom" ON "SiteDirectory"."QuantityKindFactor" ("ValidFrom");
CREATE INDEX "Idx_QuantityKindFactor_ValidTo" ON "SiteDirectory"."QuantityKindFactor" ("ValidTo");

CREATE TABLE "SiteDirectory"."QuantityKindFactor_Audit" (LIKE "SiteDirectory"."QuantityKindFactor");

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."QuantityKindFactor_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_QuantityKindFactorAudit_ValidFrom" ON "SiteDirectory"."QuantityKindFactor_Audit" ("ValidFrom");
CREATE INDEX "Idx_QuantityKindFactorAudit_ValidTo" ON "SiteDirectory"."QuantityKindFactor_Audit" ("ValidTo");

CREATE TRIGGER QuantityKindFactor_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."QuantityKindFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER QuantityKindFactor_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."QuantityKindFactor"
  FOR EACH ROW
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."MeasurementScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_MeasurementScale_ValidFrom" ON "SiteDirectory"."MeasurementScale" ("ValidFrom");
CREATE INDEX "Idx_MeasurementScale_ValidTo" ON "SiteDirectory"."MeasurementScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."MeasurementScale_Audit" (LIKE "SiteDirectory"."MeasurementScale");

ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."MeasurementScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MeasurementScaleAudit_ValidFrom" ON "SiteDirectory"."MeasurementScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_MeasurementScaleAudit_ValidTo" ON "SiteDirectory"."MeasurementScale_Audit" ("ValidTo");

CREATE TRIGGER MeasurementScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MeasurementScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MeasurementScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MeasurementScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."OrdinalScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_OrdinalScale_ValidFrom" ON "SiteDirectory"."OrdinalScale" ("ValidFrom");
CREATE INDEX "Idx_OrdinalScale_ValidTo" ON "SiteDirectory"."OrdinalScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."OrdinalScale_Audit" (LIKE "SiteDirectory"."OrdinalScale");

ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."OrdinalScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_OrdinalScaleAudit_ValidFrom" ON "SiteDirectory"."OrdinalScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_OrdinalScaleAudit_ValidTo" ON "SiteDirectory"."OrdinalScale_Audit" ("ValidTo");

CREATE TRIGGER OrdinalScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."OrdinalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER OrdinalScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."OrdinalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ScaleValueDefinition"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ScaleValueDefinition_ValidFrom" ON "SiteDirectory"."ScaleValueDefinition" ("ValidFrom");
CREATE INDEX "Idx_ScaleValueDefinition_ValidTo" ON "SiteDirectory"."ScaleValueDefinition" ("ValidTo");

CREATE TABLE "SiteDirectory"."ScaleValueDefinition_Audit" (LIKE "SiteDirectory"."ScaleValueDefinition");

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleValueDefinition_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ScaleValueDefinitionAudit_ValidFrom" ON "SiteDirectory"."ScaleValueDefinition_Audit" ("ValidFrom");
CREATE INDEX "Idx_ScaleValueDefinitionAudit_ValidTo" ON "SiteDirectory"."ScaleValueDefinition_Audit" ("ValidTo");

CREATE TRIGGER ScaleValueDefinition_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ScaleValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ScaleValueDefinition_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ScaleValueDefinition"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."MappingToReferenceScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_MappingToReferenceScale_ValidFrom" ON "SiteDirectory"."MappingToReferenceScale" ("ValidFrom");
CREATE INDEX "Idx_MappingToReferenceScale_ValidTo" ON "SiteDirectory"."MappingToReferenceScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."MappingToReferenceScale_Audit" (LIKE "SiteDirectory"."MappingToReferenceScale");

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."MappingToReferenceScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MappingToReferenceScaleAudit_ValidFrom" ON "SiteDirectory"."MappingToReferenceScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_MappingToReferenceScaleAudit_ValidTo" ON "SiteDirectory"."MappingToReferenceScale_Audit" ("ValidTo");

CREATE TRIGGER MappingToReferenceScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MappingToReferenceScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MappingToReferenceScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MappingToReferenceScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."RatioScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_RatioScale_ValidFrom" ON "SiteDirectory"."RatioScale" ("ValidFrom");
CREATE INDEX "Idx_RatioScale_ValidTo" ON "SiteDirectory"."RatioScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."RatioScale_Audit" (LIKE "SiteDirectory"."RatioScale");

ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."RatioScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."RatioScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RatioScaleAudit_ValidFrom" ON "SiteDirectory"."RatioScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_RatioScaleAudit_ValidTo" ON "SiteDirectory"."RatioScale_Audit" ("ValidTo");

CREATE TRIGGER RatioScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."RatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER RatioScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."RatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."CyclicRatioScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_CyclicRatioScale_ValidFrom" ON "SiteDirectory"."CyclicRatioScale" ("ValidFrom");
CREATE INDEX "Idx_CyclicRatioScale_ValidTo" ON "SiteDirectory"."CyclicRatioScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."CyclicRatioScale_Audit" (LIKE "SiteDirectory"."CyclicRatioScale");

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."CyclicRatioScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_CyclicRatioScaleAudit_ValidFrom" ON "SiteDirectory"."CyclicRatioScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_CyclicRatioScaleAudit_ValidTo" ON "SiteDirectory"."CyclicRatioScale_Audit" ("ValidTo");

CREATE TRIGGER CyclicRatioScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."CyclicRatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER CyclicRatioScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."CyclicRatioScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."IntervalScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_IntervalScale_ValidFrom" ON "SiteDirectory"."IntervalScale" ("ValidFrom");
CREATE INDEX "Idx_IntervalScale_ValidTo" ON "SiteDirectory"."IntervalScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."IntervalScale_Audit" (LIKE "SiteDirectory"."IntervalScale");

ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IntervalScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."IntervalScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IntervalScaleAudit_ValidFrom" ON "SiteDirectory"."IntervalScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_IntervalScaleAudit_ValidTo" ON "SiteDirectory"."IntervalScale_Audit" ("ValidTo");

CREATE TRIGGER IntervalScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."IntervalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IntervalScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."IntervalScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."LogarithmicScale"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_LogarithmicScale_ValidFrom" ON "SiteDirectory"."LogarithmicScale" ("ValidFrom");
CREATE INDEX "Idx_LogarithmicScale_ValidTo" ON "SiteDirectory"."LogarithmicScale" ("ValidTo");

CREATE TABLE "SiteDirectory"."LogarithmicScale_Audit" (LIKE "SiteDirectory"."LogarithmicScale");

ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."LogarithmicScale_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LogarithmicScaleAudit_ValidFrom" ON "SiteDirectory"."LogarithmicScale_Audit" ("ValidFrom");
CREATE INDEX "Idx_LogarithmicScaleAudit_ValidTo" ON "SiteDirectory"."LogarithmicScale_Audit" ("ValidTo");

CREATE TRIGGER LogarithmicScale_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LogarithmicScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LogarithmicScale_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LogarithmicScale"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ScaleReferenceQuantityValue_ValidFrom" ON "SiteDirectory"."ScaleReferenceQuantityValue" ("ValidFrom");
CREATE INDEX "Idx_ScaleReferenceQuantityValue_ValidTo" ON "SiteDirectory"."ScaleReferenceQuantityValue" ("ValidTo");

CREATE TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" (LIKE "SiteDirectory"."ScaleReferenceQuantityValue");

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ScaleReferenceQuantityValue_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ScaleReferenceQuantityValueAudit_ValidFrom" ON "SiteDirectory"."ScaleReferenceQuantityValue_Audit" ("ValidFrom");
CREATE INDEX "Idx_ScaleReferenceQuantityValueAudit_ValidTo" ON "SiteDirectory"."ScaleReferenceQuantityValue_Audit" ("ValidTo");

CREATE TRIGGER ScaleReferenceQuantityValue_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ScaleReferenceQuantityValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ScaleReferenceQuantityValue_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ScaleReferenceQuantityValue"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."UnitPrefix"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_UnitPrefix_ValidFrom" ON "SiteDirectory"."UnitPrefix" ("ValidFrom");
CREATE INDEX "Idx_UnitPrefix_ValidTo" ON "SiteDirectory"."UnitPrefix" ("ValidTo");

CREATE TABLE "SiteDirectory"."UnitPrefix_Audit" (LIKE "SiteDirectory"."UnitPrefix");

ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitPrefix_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UnitPrefixAudit_ValidFrom" ON "SiteDirectory"."UnitPrefix_Audit" ("ValidFrom");
CREATE INDEX "Idx_UnitPrefixAudit_ValidTo" ON "SiteDirectory"."UnitPrefix_Audit" ("ValidTo");

CREATE TRIGGER UnitPrefix_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."UnitPrefix"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UnitPrefix_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."UnitPrefix"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."MeasurementUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_MeasurementUnit_ValidFrom" ON "SiteDirectory"."MeasurementUnit" ("ValidFrom");
CREATE INDEX "Idx_MeasurementUnit_ValidTo" ON "SiteDirectory"."MeasurementUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."MeasurementUnit_Audit" (LIKE "SiteDirectory"."MeasurementUnit");

ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."MeasurementUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MeasurementUnitAudit_ValidFrom" ON "SiteDirectory"."MeasurementUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_MeasurementUnitAudit_ValidTo" ON "SiteDirectory"."MeasurementUnit_Audit" ("ValidTo");

CREATE TRIGGER MeasurementUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MeasurementUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MeasurementUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MeasurementUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DerivedUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DerivedUnit_ValidFrom" ON "SiteDirectory"."DerivedUnit" ("ValidFrom");
CREATE INDEX "Idx_DerivedUnit_ValidTo" ON "SiteDirectory"."DerivedUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."DerivedUnit_Audit" (LIKE "SiteDirectory"."DerivedUnit");

ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DerivedUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DerivedUnitAudit_ValidFrom" ON "SiteDirectory"."DerivedUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_DerivedUnitAudit_ValidTo" ON "SiteDirectory"."DerivedUnit_Audit" ("ValidTo");

CREATE TRIGGER DerivedUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DerivedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DerivedUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DerivedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."UnitFactor"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_UnitFactor_ValidFrom" ON "SiteDirectory"."UnitFactor" ("ValidFrom");
CREATE INDEX "Idx_UnitFactor_ValidTo" ON "SiteDirectory"."UnitFactor" ("ValidTo");

CREATE TABLE "SiteDirectory"."UnitFactor_Audit" (LIKE "SiteDirectory"."UnitFactor");

ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."UnitFactor_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."UnitFactor_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_UnitFactorAudit_ValidFrom" ON "SiteDirectory"."UnitFactor_Audit" ("ValidFrom");
CREATE INDEX "Idx_UnitFactorAudit_ValidTo" ON "SiteDirectory"."UnitFactor_Audit" ("ValidTo");

CREATE TRIGGER UnitFactor_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."UnitFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER UnitFactor_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."UnitFactor"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ConversionBasedUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ConversionBasedUnit_ValidFrom" ON "SiteDirectory"."ConversionBasedUnit" ("ValidFrom");
CREATE INDEX "Idx_ConversionBasedUnit_ValidTo" ON "SiteDirectory"."ConversionBasedUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."ConversionBasedUnit_Audit" (LIKE "SiteDirectory"."ConversionBasedUnit");

ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ConversionBasedUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ConversionBasedUnitAudit_ValidFrom" ON "SiteDirectory"."ConversionBasedUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_ConversionBasedUnitAudit_ValidTo" ON "SiteDirectory"."ConversionBasedUnit_Audit" ("ValidTo");

CREATE TRIGGER ConversionBasedUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ConversionBasedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ConversionBasedUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ConversionBasedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."LinearConversionUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_LinearConversionUnit_ValidFrom" ON "SiteDirectory"."LinearConversionUnit" ("ValidFrom");
CREATE INDEX "Idx_LinearConversionUnit_ValidTo" ON "SiteDirectory"."LinearConversionUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."LinearConversionUnit_Audit" (LIKE "SiteDirectory"."LinearConversionUnit");

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."LinearConversionUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_LinearConversionUnitAudit_ValidFrom" ON "SiteDirectory"."LinearConversionUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_LinearConversionUnitAudit_ValidTo" ON "SiteDirectory"."LinearConversionUnit_Audit" ("ValidTo");

CREATE TRIGGER LinearConversionUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."LinearConversionUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER LinearConversionUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."LinearConversionUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."PrefixedUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_PrefixedUnit_ValidFrom" ON "SiteDirectory"."PrefixedUnit" ("ValidFrom");
CREATE INDEX "Idx_PrefixedUnit_ValidTo" ON "SiteDirectory"."PrefixedUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."PrefixedUnit_Audit" (LIKE "SiteDirectory"."PrefixedUnit");

ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."PrefixedUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PrefixedUnitAudit_ValidFrom" ON "SiteDirectory"."PrefixedUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_PrefixedUnitAudit_ValidTo" ON "SiteDirectory"."PrefixedUnit_Audit" ("ValidTo");

CREATE TRIGGER PrefixedUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."PrefixedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PrefixedUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."PrefixedUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SimpleUnit"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SimpleUnit_ValidFrom" ON "SiteDirectory"."SimpleUnit" ("ValidFrom");
CREATE INDEX "Idx_SimpleUnit_ValidTo" ON "SiteDirectory"."SimpleUnit" ("ValidTo");

CREATE TABLE "SiteDirectory"."SimpleUnit_Audit" (LIKE "SiteDirectory"."SimpleUnit");

ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SimpleUnit_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SimpleUnitAudit_ValidFrom" ON "SiteDirectory"."SimpleUnit_Audit" ("ValidFrom");
CREATE INDEX "Idx_SimpleUnitAudit_ValidTo" ON "SiteDirectory"."SimpleUnit_Audit" ("ValidTo");

CREATE TRIGGER SimpleUnit_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SimpleUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SimpleUnit_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SimpleUnit"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."FileType"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_FileType_ValidFrom" ON "SiteDirectory"."FileType" ("ValidFrom");
CREATE INDEX "Idx_FileType_ValidTo" ON "SiteDirectory"."FileType" ("ValidTo");

CREATE TABLE "SiteDirectory"."FileType_Audit" (LIKE "SiteDirectory"."FileType");

ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."FileType_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."FileType_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_FileTypeAudit_ValidFrom" ON "SiteDirectory"."FileType_Audit" ("ValidFrom");
CREATE INDEX "Idx_FileTypeAudit_ValidTo" ON "SiteDirectory"."FileType_Audit" ("ValidTo");

CREATE TRIGGER FileType_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER FileType_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."FileType"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Glossary"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Glossary_ValidFrom" ON "SiteDirectory"."Glossary" ("ValidFrom");
CREATE INDEX "Idx_Glossary_ValidTo" ON "SiteDirectory"."Glossary" ("ValidTo");

CREATE TABLE "SiteDirectory"."Glossary_Audit" (LIKE "SiteDirectory"."Glossary");

ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Glossary_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Glossary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_GlossaryAudit_ValidFrom" ON "SiteDirectory"."Glossary_Audit" ("ValidFrom");
CREATE INDEX "Idx_GlossaryAudit_ValidTo" ON "SiteDirectory"."Glossary_Audit" ("ValidTo");

CREATE TRIGGER Glossary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Glossary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Glossary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Glossary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Term"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Term_ValidFrom" ON "SiteDirectory"."Term" ("ValidFrom");
CREATE INDEX "Idx_Term_ValidTo" ON "SiteDirectory"."Term" ("ValidTo");

CREATE TABLE "SiteDirectory"."Term_Audit" (LIKE "SiteDirectory"."Term");

ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Term_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Term_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_TermAudit_ValidFrom" ON "SiteDirectory"."Term_Audit" ("ValidFrom");
CREATE INDEX "Idx_TermAudit_ValidTo" ON "SiteDirectory"."Term_Audit" ("ValidTo");

CREATE TRIGGER Term_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Term_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Term"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ReferenceSource"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferenceSource_ValidFrom" ON "SiteDirectory"."ReferenceSource" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSource_ValidTo" ON "SiteDirectory"."ReferenceSource" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferenceSource_Audit" (LIKE "SiteDirectory"."ReferenceSource");

ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferenceSource_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferenceSourceAudit_ValidFrom" ON "SiteDirectory"."ReferenceSource_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferenceSourceAudit_ValidTo" ON "SiteDirectory"."ReferenceSource_Audit" ("ValidTo");

CREATE TRIGGER ReferenceSource_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferenceSource"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferenceSource_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferenceSource"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Rule"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Rule_ValidFrom" ON "SiteDirectory"."Rule" ("ValidFrom");
CREATE INDEX "Idx_Rule_ValidTo" ON "SiteDirectory"."Rule" ("ValidTo");

CREATE TABLE "SiteDirectory"."Rule_Audit" (LIKE "SiteDirectory"."Rule");

ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Rule_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Rule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_RuleAudit_ValidFrom" ON "SiteDirectory"."Rule_Audit" ("ValidFrom");
CREATE INDEX "Idx_RuleAudit_ValidTo" ON "SiteDirectory"."Rule_Audit" ("ValidTo");

CREATE TRIGGER Rule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Rule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Rule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Rule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ReferencerRule"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ReferencerRule_ValidFrom" ON "SiteDirectory"."ReferencerRule" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRule_ValidTo" ON "SiteDirectory"."ReferencerRule" ("ValidTo");

CREATE TABLE "SiteDirectory"."ReferencerRule_Audit" (LIKE "SiteDirectory"."ReferencerRule");

ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ReferencerRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ReferencerRuleAudit_ValidFrom" ON "SiteDirectory"."ReferencerRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_ReferencerRuleAudit_ValidTo" ON "SiteDirectory"."ReferencerRule_Audit" ("ValidTo");

CREATE TRIGGER ReferencerRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ReferencerRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ReferencerRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ReferencerRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."BinaryRelationshipRule"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_BinaryRelationshipRule_ValidFrom" ON "SiteDirectory"."BinaryRelationshipRule" ("ValidFrom");
CREATE INDEX "Idx_BinaryRelationshipRule_ValidTo" ON "SiteDirectory"."BinaryRelationshipRule" ("ValidTo");

CREATE TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" (LIKE "SiteDirectory"."BinaryRelationshipRule");

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."BinaryRelationshipRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_BinaryRelationshipRuleAudit_ValidFrom" ON "SiteDirectory"."BinaryRelationshipRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_BinaryRelationshipRuleAudit_ValidTo" ON "SiteDirectory"."BinaryRelationshipRule_Audit" ("ValidTo");

CREATE TRIGGER BinaryRelationshipRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."BinaryRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER BinaryRelationshipRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."BinaryRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."MultiRelationshipRule"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_MultiRelationshipRule_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRule_ValidTo" ON "SiteDirectory"."MultiRelationshipRule" ("ValidTo");

CREATE TABLE "SiteDirectory"."MultiRelationshipRule_Audit" (LIKE "SiteDirectory"."MultiRelationshipRule");

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."MultiRelationshipRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_MultiRelationshipRuleAudit_ValidFrom" ON "SiteDirectory"."MultiRelationshipRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_MultiRelationshipRuleAudit_ValidTo" ON "SiteDirectory"."MultiRelationshipRule_Audit" ("ValidTo");

CREATE TRIGGER MultiRelationshipRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."MultiRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER MultiRelationshipRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."MultiRelationshipRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DecompositionRule"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DecompositionRule_ValidFrom" ON "SiteDirectory"."DecompositionRule" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRule_ValidTo" ON "SiteDirectory"."DecompositionRule" ("ValidTo");

CREATE TABLE "SiteDirectory"."DecompositionRule_Audit" (LIKE "SiteDirectory"."DecompositionRule");

ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DecompositionRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DecompositionRuleAudit_ValidFrom" ON "SiteDirectory"."DecompositionRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_DecompositionRuleAudit_ValidTo" ON "SiteDirectory"."DecompositionRule_Audit" ("ValidTo");

CREATE TRIGGER DecompositionRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DecompositionRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DecompositionRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DecompositionRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ParameterizedCategoryRule_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRule_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule" ("ValidTo");

CREATE TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" (LIKE "SiteDirectory"."ParameterizedCategoryRule");

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ParameterizedCategoryRule_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParameterizedCategoryRuleAudit_ValidFrom" ON "SiteDirectory"."ParameterizedCategoryRule_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParameterizedCategoryRuleAudit_ValidTo" ON "SiteDirectory"."ParameterizedCategoryRule_Audit" ("ValidTo");

CREATE TRIGGER ParameterizedCategoryRule_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ParameterizedCategoryRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ParameterizedCategoryRule_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ParameterizedCategoryRule"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Constant"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Constant_ValidFrom" ON "SiteDirectory"."Constant" ("ValidFrom");
CREATE INDEX "Idx_Constant_ValidTo" ON "SiteDirectory"."Constant" ("ValidTo");

CREATE TABLE "SiteDirectory"."Constant_Audit" (LIKE "SiteDirectory"."Constant");

ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Constant_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Constant_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ConstantAudit_ValidFrom" ON "SiteDirectory"."Constant_Audit" ("ValidFrom");
CREATE INDEX "Idx_ConstantAudit_ValidTo" ON "SiteDirectory"."Constant_Audit" ("ValidTo");

CREATE TRIGGER Constant_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Constant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Constant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Constant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."EngineeringModelSetup"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_EngineeringModelSetup_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetup_ValidTo" ON "SiteDirectory"."EngineeringModelSetup" ("ValidTo");

CREATE TABLE "SiteDirectory"."EngineeringModelSetup_Audit" (LIKE "SiteDirectory"."EngineeringModelSetup");

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."EngineeringModelSetup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_EngineeringModelSetupAudit_ValidFrom" ON "SiteDirectory"."EngineeringModelSetup_Audit" ("ValidFrom");
CREATE INDEX "Idx_EngineeringModelSetupAudit_ValidTo" ON "SiteDirectory"."EngineeringModelSetup_Audit" ("ValidTo");

CREATE TRIGGER EngineeringModelSetup_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."EngineeringModelSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER EngineeringModelSetup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."EngineeringModelSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."Participant"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_Participant_ValidFrom" ON "SiteDirectory"."Participant" ("ValidFrom");
CREATE INDEX "Idx_Participant_ValidTo" ON "SiteDirectory"."Participant" ("ValidTo");

CREATE TABLE "SiteDirectory"."Participant_Audit" (LIKE "SiteDirectory"."Participant");

ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."Participant_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."Participant_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ParticipantAudit_ValidFrom" ON "SiteDirectory"."Participant_Audit" ("ValidFrom");
CREATE INDEX "Idx_ParticipantAudit_ValidTo" ON "SiteDirectory"."Participant_Audit" ("ValidTo");

CREATE TRIGGER Participant_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."Participant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER Participant_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."Participant"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ModelReferenceDataLibrary_ValidFrom" ON "SiteDirectory"."ModelReferenceDataLibrary" ("ValidFrom");
CREATE INDEX "Idx_ModelReferenceDataLibrary_ValidTo" ON "SiteDirectory"."ModelReferenceDataLibrary" ("ValidTo");

CREATE TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" (LIKE "SiteDirectory"."ModelReferenceDataLibrary");

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ModelReferenceDataLibrary_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ModelReferenceDataLibraryAudit_ValidFrom" ON "SiteDirectory"."ModelReferenceDataLibrary_Audit" ("ValidFrom");
CREATE INDEX "Idx_ModelReferenceDataLibraryAudit_ValidTo" ON "SiteDirectory"."ModelReferenceDataLibrary_Audit" ("ValidTo");

CREATE TRIGGER ModelReferenceDataLibrary_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ModelReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ModelReferenceDataLibrary_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ModelReferenceDataLibrary"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."IterationSetup"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_IterationSetup_ValidFrom" ON "SiteDirectory"."IterationSetup" ("ValidFrom");
CREATE INDEX "Idx_IterationSetup_ValidTo" ON "SiteDirectory"."IterationSetup" ("ValidTo");

CREATE TABLE "SiteDirectory"."IterationSetup_Audit" (LIKE "SiteDirectory"."IterationSetup");

ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."IterationSetup_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."IterationSetup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_IterationSetupAudit_ValidFrom" ON "SiteDirectory"."IterationSetup_Audit" ("ValidFrom");
CREATE INDEX "Idx_IterationSetupAudit_ValidTo" ON "SiteDirectory"."IterationSetup_Audit" ("ValidTo");

CREATE TRIGGER IterationSetup_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."IterationSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER IterationSetup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."IterationSetup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."PersonRole"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_PersonRole_ValidFrom" ON "SiteDirectory"."PersonRole" ("ValidFrom");
CREATE INDEX "Idx_PersonRole_ValidTo" ON "SiteDirectory"."PersonRole" ("ValidTo");

CREATE TABLE "SiteDirectory"."PersonRole_Audit" (LIKE "SiteDirectory"."PersonRole");

ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonRole_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonRole_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PersonRoleAudit_ValidFrom" ON "SiteDirectory"."PersonRole_Audit" ("ValidFrom");
CREATE INDEX "Idx_PersonRoleAudit_ValidTo" ON "SiteDirectory"."PersonRole_Audit" ("ValidTo");

CREATE TRIGGER PersonRole_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."PersonRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PersonRole_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."PersonRole"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."PersonPermission"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_PersonPermission_ValidFrom" ON "SiteDirectory"."PersonPermission" ("ValidFrom");
CREATE INDEX "Idx_PersonPermission_ValidTo" ON "SiteDirectory"."PersonPermission" ("ValidTo");

CREATE TABLE "SiteDirectory"."PersonPermission_Audit" (LIKE "SiteDirectory"."PersonPermission");

ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."PersonPermission_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."PersonPermission_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_PersonPermissionAudit_ValidFrom" ON "SiteDirectory"."PersonPermission_Audit" ("ValidFrom");
CREATE INDEX "Idx_PersonPermissionAudit_ValidTo" ON "SiteDirectory"."PersonPermission_Audit" ("ValidTo");

CREATE TRIGGER PersonPermission_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."PersonPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER PersonPermission_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."PersonPermission"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SiteLogEntry"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteLogEntry_ValidFrom" ON "SiteDirectory"."SiteLogEntry" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntry_ValidTo" ON "SiteDirectory"."SiteLogEntry" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteLogEntry_Audit" (LIKE "SiteDirectory"."SiteLogEntry");

ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteLogEntry_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteLogEntryAudit_ValidFrom" ON "SiteDirectory"."SiteLogEntry_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteLogEntryAudit_ValidTo" ON "SiteDirectory"."SiteLogEntry_Audit" ("ValidTo");

CREATE TRIGGER SiteLogEntry_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteLogEntry"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteLogEntry_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteLogEntry"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DomainOfExpertiseGroup_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroup_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup" ("ValidTo");

CREATE TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" (LIKE "SiteDirectory"."DomainOfExpertiseGroup");

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertiseGroup_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertiseGroupAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertiseGroup_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseGroupAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertiseGroup_Audit" ("ValidTo");

CREATE TRIGGER DomainOfExpertiseGroup_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertiseGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertiseGroup_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertiseGroup"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DomainOfExpertise"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DomainOfExpertise_ValidFrom" ON "SiteDirectory"."DomainOfExpertise" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertise_ValidTo" ON "SiteDirectory"."DomainOfExpertise" ("ValidTo");

CREATE TABLE "SiteDirectory"."DomainOfExpertise_Audit" (LIKE "SiteDirectory"."DomainOfExpertise");

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DomainOfExpertise_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DomainOfExpertiseAudit_ValidFrom" ON "SiteDirectory"."DomainOfExpertise_Audit" ("ValidFrom");
CREATE INDEX "Idx_DomainOfExpertiseAudit_ValidTo" ON "SiteDirectory"."DomainOfExpertise_Audit" ("ValidTo");

CREATE TRIGGER DomainOfExpertise_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DomainOfExpertise"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DomainOfExpertise_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DomainOfExpertise"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."NaturalLanguage"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_NaturalLanguage_ValidFrom" ON "SiteDirectory"."NaturalLanguage" ("ValidFrom");
CREATE INDEX "Idx_NaturalLanguage_ValidTo" ON "SiteDirectory"."NaturalLanguage" ("ValidTo");

CREATE TABLE "SiteDirectory"."NaturalLanguage_Audit" (LIKE "SiteDirectory"."NaturalLanguage");

ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."NaturalLanguage_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_NaturalLanguageAudit_ValidFrom" ON "SiteDirectory"."NaturalLanguage_Audit" ("ValidFrom");
CREATE INDEX "Idx_NaturalLanguageAudit_ValidTo" ON "SiteDirectory"."NaturalLanguage_Audit" ("ValidTo");

CREATE TRIGGER NaturalLanguage_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."NaturalLanguage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER NaturalLanguage_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."NaturalLanguage"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."GenericAnnotation"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_GenericAnnotation_ValidFrom" ON "SiteDirectory"."GenericAnnotation" ("ValidFrom");
CREATE INDEX "Idx_GenericAnnotation_ValidTo" ON "SiteDirectory"."GenericAnnotation" ("ValidTo");

CREATE TABLE "SiteDirectory"."GenericAnnotation_Audit" (LIKE "SiteDirectory"."GenericAnnotation");

ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."GenericAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_GenericAnnotationAudit_ValidFrom" ON "SiteDirectory"."GenericAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_GenericAnnotationAudit_ValidTo" ON "SiteDirectory"."GenericAnnotation_Audit" ("ValidTo");

CREATE TRIGGER GenericAnnotation_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."GenericAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER GenericAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."GenericAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteDirectoryDataAnnotation_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataAnnotation" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataAnnotation_ValidTo" ON "SiteDirectory"."SiteDirectoryDataAnnotation" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" (LIKE "SiteDirectory"."SiteDirectoryDataAnnotation");

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryDataAnnotationAudit_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataAnnotationAudit_ValidTo" ON "SiteDirectory"."SiteDirectoryDataAnnotation_Audit" ("ValidTo");

CREATE TRIGGER SiteDirectoryDataAnnotation_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectoryDataAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectoryDataAnnotation_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectoryDataAnnotation"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."ThingReference"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_ThingReference_ValidFrom" ON "SiteDirectory"."ThingReference" ("ValidFrom");
CREATE INDEX "Idx_ThingReference_ValidTo" ON "SiteDirectory"."ThingReference" ("ValidTo");

CREATE TABLE "SiteDirectory"."ThingReference_Audit" (LIKE "SiteDirectory"."ThingReference");

ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."ThingReference_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."ThingReference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_ThingReferenceAudit_ValidFrom" ON "SiteDirectory"."ThingReference_Audit" ("ValidFrom");
CREATE INDEX "Idx_ThingReferenceAudit_ValidTo" ON "SiteDirectory"."ThingReference_Audit" ("ValidTo");

CREATE TRIGGER ThingReference_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."ThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER ThingReference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."ThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteDirectoryThingReference_ValidFrom" ON "SiteDirectory"."SiteDirectoryThingReference" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryThingReference_ValidTo" ON "SiteDirectory"."SiteDirectoryThingReference" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" (LIKE "SiteDirectory"."SiteDirectoryThingReference");

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryThingReference_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryThingReferenceAudit_ValidFrom" ON "SiteDirectory"."SiteDirectoryThingReference_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryThingReferenceAudit_ValidTo" ON "SiteDirectory"."SiteDirectoryThingReference_Audit" ("ValidTo");

CREATE TRIGGER SiteDirectoryThingReference_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectoryThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectoryThingReference_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectoryThingReference"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."DiscussionItem"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_DiscussionItem_ValidFrom" ON "SiteDirectory"."DiscussionItem" ("ValidFrom");
CREATE INDEX "Idx_DiscussionItem_ValidTo" ON "SiteDirectory"."DiscussionItem" ("ValidTo");

CREATE TABLE "SiteDirectory"."DiscussionItem_Audit" (LIKE "SiteDirectory"."DiscussionItem");

ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."DiscussionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_DiscussionItemAudit_ValidFrom" ON "SiteDirectory"."DiscussionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_DiscussionItemAudit_ValidTo" ON "SiteDirectory"."DiscussionItem_Audit" ("ValidTo");

CREATE TRIGGER DiscussionItem_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."DiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER DiscussionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."DiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  ADD COLUMN "ValidFrom" timestamp DEFAULT "SiteDirectory".get_transaction_time() NOT NULL,
  ADD COLUMN "ValidTo" timestamp DEFAULT 'infinity' NOT NULL;
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItem_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItem_ValidTo" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem" ("ValidTo");

CREATE TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" (LIKE "SiteDirectory"."SiteDirectoryDataDiscussionItem");

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_vacuum_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_vacuum_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_analyze_scale_factor = 0.0);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" SET (autovacuum_analyze_threshold = 2500);

ALTER TABLE "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" 
  ADD COLUMN "Action" character(1) NOT NULL,
  ADD COLUMN "Actor" uuid;
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItemAudit_ValidFrom" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" ("ValidFrom");
CREATE INDEX "Idx_SiteDirectoryDataDiscussionItemAudit_ValidTo" ON "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit" ("ValidTo");

CREATE TRIGGER SiteDirectoryDataDiscussionItem_audit_prepare
  BEFORE UPDATE ON "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_before();

CREATE TRIGGER SiteDirectoryDataDiscussionItem_audit_log
  AFTER INSERT OR UPDATE OR DELETE ON "SiteDirectory"."SiteDirectoryDataDiscussionItem"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SiteDirectory".process_timetravel_after();
CREATE OR REPLACE FUNCTION "SiteDirectory"."Thing_Data" ()
    RETURNS SETOF "SiteDirectory"."Thing" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Thing";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Thing"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Thing_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."TopContainer_Data" ()
    RETURNS SETOF "SiteDirectory"."TopContainer" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."TopContainer";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."TopContainer"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."TopContainer_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteDirectory_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteDirectory" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteDirectory";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","DefaultParticipantRole","DefaultPersonRole","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteDirectory"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","DefaultParticipantRole","DefaultPersonRole","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteDirectory_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Organization_Data" ()
    RETURNS SETOF "SiteDirectory"."Organization" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Organization";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Organization"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Organization_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Person_Data" ()
    RETURNS SETOF "SiteDirectory"."Person" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Person";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Organization","DefaultDomain","Role","DefaultEmailAddress","DefaultTelephoneNumber","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Person"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Organization","DefaultDomain","Role","DefaultEmailAddress","DefaultTelephoneNumber","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Person_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."EmailAddress_Data" ()
    RETURNS SETOF "SiteDirectory"."EmailAddress" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."EmailAddress";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."EmailAddress"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."EmailAddress_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."TelephoneNumber_Data" ()
    RETURNS SETOF "SiteDirectory"."TelephoneNumber" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."TelephoneNumber";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."TelephoneNumber"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."TelephoneNumber_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."UserPreference_Data" ()
    RETURNS SETOF "SiteDirectory"."UserPreference" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."UserPreference";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."UserPreference"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."UserPreference_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DefinedThing_Data" ()
    RETURNS SETOF "SiteDirectory"."DefinedThing" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DefinedThing";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DefinedThing"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DefinedThing_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParticipantRole_Data" ()
    RETURNS SETOF "SiteDirectory"."ParticipantRole" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParticipantRole";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParticipantRole"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParticipantRole_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Alias_Data" ()
    RETURNS SETOF "SiteDirectory"."Alias" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Alias";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Alias"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Alias_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Definition_Data" ()
    RETURNS SETOF "SiteDirectory"."Definition" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Definition";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Definition"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Definition_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Citation_Data" ()
    RETURNS SETOF "SiteDirectory"."Citation" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Citation";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Source","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Citation"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Source","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Citation_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."HyperLink_Data" ()
    RETURNS SETOF "SiteDirectory"."HyperLink" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."HyperLink";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."HyperLink"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."HyperLink_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParticipantPermission_Data" ()
    RETURNS SETOF "SiteDirectory"."ParticipantPermission" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParticipantPermission";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParticipantPermission"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParticipantPermission_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferenceDataLibrary_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferenceDataLibrary" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferenceDataLibrary";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","RequiredRdl","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferenceDataLibrary"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","RequiredRdl","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferenceDataLibrary_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteReferenceDataLibrary_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteReferenceDataLibrary" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteReferenceDataLibrary";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteReferenceDataLibrary"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteReferenceDataLibrary_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Category_Data" ()
    RETURNS SETOF "SiteDirectory"."Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."ParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."CompoundParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."CompoundParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."CompoundParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."CompoundParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."CompoundParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ArrayParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."ArrayParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ArrayParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ArrayParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ArrayParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParameterTypeComponent_Data" ()
    RETURNS SETOF "SiteDirectory"."ParameterTypeComponent" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParameterTypeComponent";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Sequence","ParameterType","Scale","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParameterTypeComponent"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Sequence","ParameterType","Scale","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParameterTypeComponent_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ScalarParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."ScalarParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ScalarParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ScalarParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ScalarParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."EnumerationParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."EnumerationParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."EnumerationParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."EnumerationParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."EnumerationParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."EnumerationValueDefinition_Data" ()
    RETURNS SETOF "SiteDirectory"."EnumerationValueDefinition" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."EnumerationValueDefinition";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Sequence","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."EnumerationValueDefinition"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Sequence","ValidFrom","ValidTo"
      FROM "SiteDirectory"."EnumerationValueDefinition_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."BooleanParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."BooleanParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."BooleanParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."BooleanParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."BooleanParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DateParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."DateParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DateParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DateParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DateParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."TextParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."TextParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."TextParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."TextParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."TextParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DateTimeParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."DateTimeParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DateTimeParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DateTimeParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DateTimeParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."TimeOfDayParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."TimeOfDayParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."TimeOfDayParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."TimeOfDayParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."TimeOfDayParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."QuantityKind_Data" ()
    RETURNS SETOF "SiteDirectory"."QuantityKind" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."QuantityKind";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","DefaultScale","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."QuantityKind"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","DefaultScale","ValidFrom","ValidTo"
      FROM "SiteDirectory"."QuantityKind_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SpecializedQuantityKind_Data" ()
    RETURNS SETOF "SiteDirectory"."SpecializedQuantityKind" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SpecializedQuantityKind";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","General","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SpecializedQuantityKind"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","General","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SpecializedQuantityKind_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SimpleQuantityKind_Data" ()
    RETURNS SETOF "SiteDirectory"."SimpleQuantityKind" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SimpleQuantityKind";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SimpleQuantityKind"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SimpleQuantityKind_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DerivedQuantityKind_Data" ()
    RETURNS SETOF "SiteDirectory"."DerivedQuantityKind" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DerivedQuantityKind";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DerivedQuantityKind"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DerivedQuantityKind_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."QuantityKindFactor_Data" ()
    RETURNS SETOF "SiteDirectory"."QuantityKindFactor" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."QuantityKindFactor";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Sequence","QuantityKind","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."QuantityKindFactor"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Sequence","QuantityKind","ValidFrom","ValidTo"
      FROM "SiteDirectory"."QuantityKindFactor_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."MeasurementScale_Data" ()
    RETURNS SETOF "SiteDirectory"."MeasurementScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."MeasurementScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Unit","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."MeasurementScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Unit","ValidFrom","ValidTo"
      FROM "SiteDirectory"."MeasurementScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."OrdinalScale_Data" ()
    RETURNS SETOF "SiteDirectory"."OrdinalScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."OrdinalScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."OrdinalScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."OrdinalScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ScaleValueDefinition_Data" ()
    RETURNS SETOF "SiteDirectory"."ScaleValueDefinition" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ScaleValueDefinition";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ScaleValueDefinition"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ScaleValueDefinition_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."MappingToReferenceScale_Data" ()
    RETURNS SETOF "SiteDirectory"."MappingToReferenceScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."MappingToReferenceScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ReferenceScaleValue","DependentScaleValue","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."MappingToReferenceScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ReferenceScaleValue","DependentScaleValue","ValidFrom","ValidTo"
      FROM "SiteDirectory"."MappingToReferenceScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."RatioScale_Data" ()
    RETURNS SETOF "SiteDirectory"."RatioScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."RatioScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."RatioScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."RatioScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."CyclicRatioScale_Data" ()
    RETURNS SETOF "SiteDirectory"."CyclicRatioScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."CyclicRatioScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."CyclicRatioScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."CyclicRatioScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."IntervalScale_Data" ()
    RETURNS SETOF "SiteDirectory"."IntervalScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."IntervalScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."IntervalScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."IntervalScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."LogarithmicScale_Data" ()
    RETURNS SETOF "SiteDirectory"."LogarithmicScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."LogarithmicScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ReferenceQuantityKind","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."LogarithmicScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ReferenceQuantityKind","ValidFrom","ValidTo"
      FROM "SiteDirectory"."LogarithmicScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ScaleReferenceQuantityValue_Data" ()
    RETURNS SETOF "SiteDirectory"."ScaleReferenceQuantityValue" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ScaleReferenceQuantityValue";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Scale","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ScaleReferenceQuantityValue"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Scale","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ScaleReferenceQuantityValue_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."UnitPrefix_Data" ()
    RETURNS SETOF "SiteDirectory"."UnitPrefix" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."UnitPrefix";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."UnitPrefix"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."UnitPrefix_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."MeasurementUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."MeasurementUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."MeasurementUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."MeasurementUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."MeasurementUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DerivedUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."DerivedUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DerivedUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DerivedUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DerivedUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."UnitFactor_Data" ()
    RETURNS SETOF "SiteDirectory"."UnitFactor" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."UnitFactor";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Sequence","Unit","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."UnitFactor"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Sequence","Unit","ValidFrom","ValidTo"
      FROM "SiteDirectory"."UnitFactor_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ConversionBasedUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."ConversionBasedUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ConversionBasedUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ReferenceUnit","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ConversionBasedUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ReferenceUnit","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ConversionBasedUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."LinearConversionUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."LinearConversionUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."LinearConversionUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."LinearConversionUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."LinearConversionUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."PrefixedUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."PrefixedUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."PrefixedUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Prefix","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."PrefixedUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Prefix","ValidFrom","ValidTo"
      FROM "SiteDirectory"."PrefixedUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SimpleUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."SimpleUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SimpleUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SimpleUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SimpleUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."FileType_Data" ()
    RETURNS SETOF "SiteDirectory"."FileType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."FileType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."FileType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."FileType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Glossary_Data" ()
    RETURNS SETOF "SiteDirectory"."Glossary" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Glossary";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Glossary"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Glossary_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Term_Data" ()
    RETURNS SETOF "SiteDirectory"."Term" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Term";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Term"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Term_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferenceSource_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferenceSource" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferenceSource";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Publisher","PublishedIn","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferenceSource"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Publisher","PublishedIn","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferenceSource_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Rule_Data" ()
    RETURNS SETOF "SiteDirectory"."Rule" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Rule";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Rule"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Rule_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferencerRule_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferencerRule" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferencerRule";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ReferencingCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferencerRule"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ReferencingCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferencerRule_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."BinaryRelationshipRule_Data" ()
    RETURNS SETOF "SiteDirectory"."BinaryRelationshipRule" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."BinaryRelationshipRule";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","RelationshipCategory","SourceCategory","TargetCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."BinaryRelationshipRule"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","RelationshipCategory","SourceCategory","TargetCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."BinaryRelationshipRule_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."MultiRelationshipRule_Data" ()
    RETURNS SETOF "SiteDirectory"."MultiRelationshipRule" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."MultiRelationshipRule";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","RelationshipCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."MultiRelationshipRule"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","RelationshipCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."MultiRelationshipRule_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DecompositionRule_Data" ()
    RETURNS SETOF "SiteDirectory"."DecompositionRule" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DecompositionRule";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ContainingCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DecompositionRule"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ContainingCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DecompositionRule_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParameterizedCategoryRule_Data" ()
    RETURNS SETOF "SiteDirectory"."ParameterizedCategoryRule" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParameterizedCategoryRule";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParameterizedCategoryRule"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParameterizedCategoryRule_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Constant_Data" ()
    RETURNS SETOF "SiteDirectory"."Constant" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Constant";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ParameterType","Scale","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Constant"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ParameterType","Scale","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Constant_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."EngineeringModelSetup_Data" ()
    RETURNS SETOF "SiteDirectory"."EngineeringModelSetup" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."EngineeringModelSetup";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."EngineeringModelSetup"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."EngineeringModelSetup_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Participant_Data" ()
    RETURNS SETOF "SiteDirectory"."Participant" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Participant";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Person","Role","SelectedDomain","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Participant"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Person","Role","SelectedDomain","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Participant_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ModelReferenceDataLibrary_Data" ()
    RETURNS SETOF "SiteDirectory"."ModelReferenceDataLibrary" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ModelReferenceDataLibrary";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ModelReferenceDataLibrary"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ModelReferenceDataLibrary_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."IterationSetup_Data" ()
    RETURNS SETOF "SiteDirectory"."IterationSetup" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."IterationSetup";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","SourceIterationSetup","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."IterationSetup"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","SourceIterationSetup","ValidFrom","ValidTo"
      FROM "SiteDirectory"."IterationSetup_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."PersonRole_Data" ()
    RETURNS SETOF "SiteDirectory"."PersonRole" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."PersonRole";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."PersonRole"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."PersonRole_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."PersonPermission_Data" ()
    RETURNS SETOF "SiteDirectory"."PersonPermission" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."PersonPermission";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."PersonPermission"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."PersonPermission_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteLogEntry_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteLogEntry" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteLogEntry";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Author","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteLogEntry"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Author","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteLogEntry_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DomainOfExpertiseGroup_Data" ()
    RETURNS SETOF "SiteDirectory"."DomainOfExpertiseGroup" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DomainOfExpertiseGroup";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DomainOfExpertiseGroup"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DomainOfExpertiseGroup_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DomainOfExpertise_Data" ()
    RETURNS SETOF "SiteDirectory"."DomainOfExpertise" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DomainOfExpertise";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DomainOfExpertise"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DomainOfExpertise_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."NaturalLanguage_Data" ()
    RETURNS SETOF "SiteDirectory"."NaturalLanguage" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."NaturalLanguage";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."NaturalLanguage"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."NaturalLanguage_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."GenericAnnotation_Data" ()
    RETURNS SETOF "SiteDirectory"."GenericAnnotation" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."GenericAnnotation";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."GenericAnnotation"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ValidFrom","ValidTo"
      FROM "SiteDirectory"."GenericAnnotation_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteDirectoryDataAnnotation_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteDirectoryDataAnnotation" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteDirectoryDataAnnotation";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Author","PrimaryAnnotatedThing","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteDirectoryDataAnnotation"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Author","PrimaryAnnotatedThing","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteDirectoryDataAnnotation_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ThingReference_Data" ()
    RETURNS SETOF "SiteDirectory"."ThingReference" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ThingReference";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ReferencedThing","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ThingReference"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ReferencedThing","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ThingReference_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteDirectoryThingReference_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteDirectoryThingReference" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteDirectoryThingReference";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteDirectoryThingReference"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteDirectoryThingReference_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DiscussionItem_Data" ()
    RETURNS SETOF "SiteDirectory"."DiscussionItem" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DiscussionItem";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","ReplyTo","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DiscussionItem"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","ReplyTo","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DiscussionItem_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteDirectoryDataDiscussionItem_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteDirectoryDataDiscussionItem" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteDirectoryDataDiscussionItem";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Iid","ValueTypeDictionary","Container","Author","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteDirectoryDataDiscussionItem"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Iid","ValueTypeDictionary","Container","Author","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteDirectoryDataDiscussionItem_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Thing_ExcludedPerson_Data" ()
    RETURNS SETOF "SiteDirectory"."Thing_ExcludedPerson" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Thing_ExcludedPerson";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Thing","ExcludedPerson","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Thing_ExcludedPerson"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Thing","ExcludedPerson","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Thing_ExcludedPerson_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Thing_ExcludedDomain_Data" ()
    RETURNS SETOF "SiteDirectory"."Thing_ExcludedDomain" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Thing_ExcludedDomain";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Thing","ExcludedDomain","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Thing_ExcludedDomain"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Thing","ExcludedDomain","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Thing_ExcludedDomain_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."TelephoneNumber_VcardType_Data" ()
    RETURNS SETOF "SiteDirectory"."TelephoneNumber_VcardType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."TelephoneNumber_VcardType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "TelephoneNumber","VcardType","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."TelephoneNumber_VcardType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "TelephoneNumber","VcardType","ValidFrom","ValidTo"
      FROM "SiteDirectory"."TelephoneNumber_VcardType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Definition_Note_Data" ()
    RETURNS SETOF "SiteDirectory"."Definition_Note" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Definition_Note";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Definition","Note","Sequence","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Definition_Note"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Definition","Note","Sequence","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Definition_Note_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Definition_Example_Data" ()
    RETURNS SETOF "SiteDirectory"."Definition_Example" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Definition_Example";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Definition","Example","Sequence","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Definition_Example"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Definition","Example","Sequence","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Definition_Example_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ReferenceDataLibrary","BaseQuantityKind","Sequence","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ReferenceDataLibrary","BaseQuantityKind","Sequence","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferenceDataLibrary_BaseQuantityKind_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferenceDataLibrary_BaseUnit" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferenceDataLibrary_BaseUnit";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ReferenceDataLibrary","BaseUnit","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferenceDataLibrary_BaseUnit"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ReferenceDataLibrary","BaseUnit","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferenceDataLibrary_BaseUnit_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Category_SuperCategory_Data" ()
    RETURNS SETOF "SiteDirectory"."Category_SuperCategory" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Category_SuperCategory";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Category","SuperCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Category_SuperCategory"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Category","SuperCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Category_SuperCategory_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Category_PermissibleClass_Data" ()
    RETURNS SETOF "SiteDirectory"."Category_PermissibleClass" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Category_PermissibleClass";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Category","PermissibleClass","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Category_PermissibleClass"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Category","PermissibleClass","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Category_PermissibleClass_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParameterType_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."ParameterType_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParameterType_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ParameterType","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParameterType_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ParameterType","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParameterType_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ArrayParameterType_Dimension_Data" ()
    RETURNS SETOF "SiteDirectory"."ArrayParameterType_Dimension" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ArrayParameterType_Dimension";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ArrayParameterType","Dimension","Sequence","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ArrayParameterType_Dimension"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ArrayParameterType","Dimension","Sequence","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ArrayParameterType_Dimension_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."QuantityKind_PossibleScale_Data" ()
    RETURNS SETOF "SiteDirectory"."QuantityKind_PossibleScale" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."QuantityKind_PossibleScale";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "QuantityKind","PossibleScale","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."QuantityKind_PossibleScale"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "QuantityKind","PossibleScale","ValidFrom","ValidTo"
      FROM "SiteDirectory"."QuantityKind_PossibleScale_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."FileType_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."FileType_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."FileType_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "FileType","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."FileType_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "FileType","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."FileType_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Glossary_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."Glossary_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Glossary_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Glossary","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Glossary_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Glossary","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Glossary_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferenceSource_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferenceSource_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferenceSource_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ReferenceSource","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferenceSource_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ReferenceSource","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferenceSource_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ReferencerRule_ReferencedCategory_Data" ()
    RETURNS SETOF "SiteDirectory"."ReferencerRule_ReferencedCategory" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ReferencerRule_ReferencedCategory";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ReferencerRule","ReferencedCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ReferencerRule_ReferencedCategory"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ReferencerRule","ReferencedCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ReferencerRule_ReferencedCategory_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Data" ()
    RETURNS SETOF "SiteDirectory"."MultiRelationshipRule_RelatedCategory" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."MultiRelationshipRule_RelatedCategory";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "MultiRelationshipRule","RelatedCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."MultiRelationshipRule_RelatedCategory"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "MultiRelationshipRule","RelatedCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."MultiRelationshipRule_RelatedCategory_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DecompositionRule_ContainedCategory_Data" ()
    RETURNS SETOF "SiteDirectory"."DecompositionRule_ContainedCategory" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DecompositionRule_ContainedCategory";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "DecompositionRule","ContainedCategory","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DecompositionRule_ContainedCategory"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "DecompositionRule","ContainedCategory","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DecompositionRule_ContainedCategory_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Data" ()
    RETURNS SETOF "SiteDirectory"."ParameterizedCategoryRule_ParameterType" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."ParameterizedCategoryRule_ParameterType";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "ParameterizedCategoryRule","ParameterType","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."ParameterizedCategoryRule_ParameterType"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "ParameterizedCategoryRule","ParameterType","ValidFrom","ValidTo"
      FROM "SiteDirectory"."ParameterizedCategoryRule_ParameterType_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Constant_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."Constant_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Constant_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Constant","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Constant_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Constant","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Constant_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Data" ()
    RETURNS SETOF "SiteDirectory"."EngineeringModelSetup_ActiveDomain" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."EngineeringModelSetup_ActiveDomain";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "EngineeringModelSetup","ActiveDomain","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."EngineeringModelSetup_ActiveDomain"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "EngineeringModelSetup","ActiveDomain","ValidFrom","ValidTo"
      FROM "SiteDirectory"."EngineeringModelSetup_ActiveDomain_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."Participant_Domain_Data" ()
    RETURNS SETOF "SiteDirectory"."Participant_Domain" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."Participant_Domain";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "Participant","Domain","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."Participant_Domain"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "Participant","Domain","ValidFrom","ValidTo"
      FROM "SiteDirectory"."Participant_Domain_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteLogEntry_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteLogEntry_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteLogEntry_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "SiteLogEntry","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteLogEntry_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "SiteLogEntry","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteLogEntry_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."SiteLogEntry_AffectedItemIid_Data" ()
    RETURNS SETOF "SiteDirectory"."SiteLogEntry_AffectedItemIid" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."SiteLogEntry_AffectedItemIid";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "SiteLogEntry","AffectedItemIid","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."SiteLogEntry_AffectedItemIid"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "SiteLogEntry","AffectedItemIid","ValidFrom","ValidTo"
      FROM "SiteDirectory"."SiteLogEntry_AffectedItemIid_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DomainOfExpertiseGroup_Domain_Data" ()
    RETURNS SETOF "SiteDirectory"."DomainOfExpertiseGroup_Domain" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DomainOfExpertiseGroup_Domain";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "DomainOfExpertiseGroup","Domain","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DomainOfExpertiseGroup_Domain"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "DomainOfExpertiseGroup","Domain","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DomainOfExpertiseGroup_Domain_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;
CREATE OR REPLACE FUNCTION "SiteDirectory"."DomainOfExpertise_Category_Data" ()
    RETURNS SETOF "SiteDirectory"."DomainOfExpertise_Category" AS
$BODY$
DECLARE
   instant timestamp;
BEGIN
   instant := "SiteDirectory".get_session_instant();

IF instant = 'infinity' THEN
   RETURN QUERY
   SELECT *
   FROM "SiteDirectory"."DomainOfExpertise_Category";
ELSE
   RETURN QUERY
   SELECT *
   FROM (SELECT "DomainOfExpertise","Category","ValidFrom","ValidTo" 
      FROM "SiteDirectory"."DomainOfExpertise_Category"
      -- prefilter union candidates
      WHERE "ValidFrom" < instant
      AND "ValidTo" >= instant
       UNION ALL
      SELECT "DomainOfExpertise","Category","ValidFrom","ValidTo"
      FROM "SiteDirectory"."DomainOfExpertise_Category_Audit"
      -- prefilter union candidates
      WHERE "Action" <> 'I'
      AND "ValidFrom" < instant
      AND "ValidTo" >= instant) "VersionedData"
   ORDER BY "VersionedData"."ValidTo" DESC;
END IF;

END
$BODY$
  LANGUAGE plpgsql VOLATILE;

CREATE VIEW "SiteDirectory"."Thing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" AS "ValueTypeSet",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."TopContainer_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TopContainer"."ValueTypeDictionary" AS "ValueTypeSet",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."SiteDirectory_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TopContainer"."ValueTypeDictionary" || "SiteDirectory"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectory"."DefaultParticipantRole",
	"SiteDirectory"."DefaultPersonRole",
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

CREATE VIEW "SiteDirectory"."Organization_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Organization"."ValueTypeDictionary" AS "ValueTypeSet",
	"Organization"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."Person_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Person"."ValueTypeDictionary" AS "ValueTypeSet",
	"Person"."Container",
	NULL::bigint AS "Sequence",
	"Person"."Organization",
	"Person"."DefaultDomain",
	"Person"."Role",
	"Person"."DefaultEmailAddress",
	"Person"."DefaultTelephoneNumber",
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

CREATE VIEW "SiteDirectory"."EmailAddress_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "EmailAddress"."ValueTypeDictionary" AS "ValueTypeSet",
	"EmailAddress"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."TelephoneNumber_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "TelephoneNumber"."ValueTypeDictionary" AS "ValueTypeSet",
	"TelephoneNumber"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "TelephoneNumber") AS "TelephoneNumber_VcardType" USING ("Iid");

CREATE VIEW "SiteDirectory"."UserPreference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "UserPreference"."ValueTypeDictionary" AS "ValueTypeSet",
	"UserPreference"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."DefinedThing_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" AS "ValueTypeSet",
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

CREATE VIEW "SiteDirectory"."ParticipantRole_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParticipantRole"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParticipantRole"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."Alias_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Alias"."ValueTypeDictionary" AS "ValueTypeSet",
	"Alias"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."Definition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Definition"."ValueTypeDictionary" AS "ValueTypeSet",
	"Definition"."Container",
	NULL::bigint AS "Sequence",
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
  LEFT JOIN (SELECT "Citation"."Container" AS "Iid", array_agg("Citation"."Iid"::text) AS "Citation"
   FROM "SiteDirectory"."Citation_Data"() AS "Citation"
   JOIN "SiteDirectory"."Definition_Data"() AS "Definition" ON "Citation"."Container" = "Definition"."Iid"
   GROUP BY "Citation"."Container") AS "Definition_Citation" USING ("Iid");

CREATE VIEW "SiteDirectory"."Citation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Citation"."ValueTypeDictionary" AS "ValueTypeSet",
	"Citation"."Container",
	NULL::bigint AS "Sequence",
	"Citation"."Source",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."HyperLink_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "HyperLink"."ValueTypeDictionary" AS "ValueTypeSet",
	"HyperLink"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."ParticipantPermission_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParticipantPermission"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParticipantPermission"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."ReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"ReferenceDataLibrary"."RequiredRdl",
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

CREATE VIEW "SiteDirectory"."SiteReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" || "SiteReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteReferenceDataLibrary"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceDataLibrary"."RequiredRdl",
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

CREATE VIEW "SiteDirectory"."Category_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Category"."ValueTypeDictionary" AS "ValueTypeSet",
	"Category"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."ParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."CompoundParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "CompoundParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."ArrayParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "CompoundParameterType"."ValueTypeDictionary" || "ArrayParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."ParameterTypeComponent_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ParameterTypeComponent"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterTypeComponent"."Container",
	"ParameterTypeComponent"."Sequence",
	"ParameterTypeComponent"."ParameterType",
	"ParameterTypeComponent"."Scale",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."ScalarParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."EnumerationParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "EnumerationParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."EnumerationValueDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EnumerationValueDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"EnumerationValueDefinition"."Container",
	"EnumerationValueDefinition"."Sequence",
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

CREATE VIEW "SiteDirectory"."BooleanParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "BooleanParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."DateParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "DateParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."TextParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "TextParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."DateTimeParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "DateTimeParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."TimeOfDayParameterType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "TimeOfDayParameterType"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."QuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
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

CREATE VIEW "SiteDirectory"."SpecializedQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "SpecializedQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
	"SpecializedQuantityKind"."General",
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

CREATE VIEW "SiteDirectory"."SimpleQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "SimpleQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
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

CREATE VIEW "SiteDirectory"."DerivedQuantityKind_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ParameterType"."ValueTypeDictionary" || "ScalarParameterType"."ValueTypeDictionary" || "QuantityKind"."ValueTypeDictionary" || "DerivedQuantityKind"."ValueTypeDictionary" AS "ValueTypeSet",
	"ParameterType"."Container",
	NULL::bigint AS "Sequence",
	"QuantityKind"."DefaultScale",
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

CREATE VIEW "SiteDirectory"."QuantityKindFactor_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "QuantityKindFactor"."ValueTypeDictionary" AS "ValueTypeSet",
	"QuantityKindFactor"."Container",
	"QuantityKindFactor"."Sequence",
	"QuantityKindFactor"."QuantityKind",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."MeasurementScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
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

CREATE VIEW "SiteDirectory"."OrdinalScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "OrdinalScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
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

CREATE VIEW "SiteDirectory"."ScaleValueDefinition_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ScaleValueDefinition"."ValueTypeDictionary" AS "ValueTypeSet",
	"ScaleValueDefinition"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."MappingToReferenceScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "MappingToReferenceScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MappingToReferenceScale"."Container",
	NULL::bigint AS "Sequence",
	"MappingToReferenceScale"."ReferenceScaleValue",
	"MappingToReferenceScale"."DependentScaleValue",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."RatioScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "RatioScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
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

CREATE VIEW "SiteDirectory"."CyclicRatioScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "RatioScale"."ValueTypeDictionary" || "CyclicRatioScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
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

CREATE VIEW "SiteDirectory"."IntervalScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "IntervalScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
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

CREATE VIEW "SiteDirectory"."LogarithmicScale_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementScale"."ValueTypeDictionary" || "LogarithmicScale"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementScale"."Container",
	NULL::bigint AS "Sequence",
	"MeasurementScale"."Unit",
	"LogarithmicScale"."ReferenceQuantityKind",
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

CREATE VIEW "SiteDirectory"."ScaleReferenceQuantityValue_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ScaleReferenceQuantityValue"."ValueTypeDictionary" AS "ValueTypeSet",
	"ScaleReferenceQuantityValue"."Container",
	NULL::bigint AS "Sequence",
	"ScaleReferenceQuantityValue"."Scale",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."UnitPrefix_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "UnitPrefix"."ValueTypeDictionary" AS "ValueTypeSet",
	"UnitPrefix"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."MeasurementUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."DerivedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "DerivedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."UnitFactor_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "UnitFactor"."ValueTypeDictionary" AS "ValueTypeSet",
	"UnitFactor"."Container",
	"UnitFactor"."Sequence",
	"UnitFactor"."Unit",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."ConversionBasedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
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

CREATE VIEW "SiteDirectory"."LinearConversionUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" || "LinearConversionUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
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

CREATE VIEW "SiteDirectory"."PrefixedUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "ConversionBasedUnit"."ValueTypeDictionary" || "PrefixedUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
	"ConversionBasedUnit"."ReferenceUnit",
	"PrefixedUnit"."Prefix",
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

CREATE VIEW "SiteDirectory"."SimpleUnit_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "MeasurementUnit"."ValueTypeDictionary" || "SimpleUnit"."ValueTypeDictionary" AS "ValueTypeSet",
	"MeasurementUnit"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."FileType_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "FileType"."ValueTypeDictionary" AS "ValueTypeSet",
	"FileType"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."Glossary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Glossary"."ValueTypeDictionary" AS "ValueTypeSet",
	"Glossary"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."Term_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Term"."ValueTypeDictionary" AS "ValueTypeSet",
	"Term"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."ReferenceSource_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceSource"."ValueTypeDictionary" AS "ValueTypeSet",
	"ReferenceSource"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceSource"."Publisher",
	"ReferenceSource"."PublishedIn",
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

CREATE VIEW "SiteDirectory"."Rule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."ReferencerRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "ReferencerRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"ReferencerRule"."ReferencingCategory",
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

CREATE VIEW "SiteDirectory"."BinaryRelationshipRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "BinaryRelationshipRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"BinaryRelationshipRule"."RelationshipCategory",
	"BinaryRelationshipRule"."SourceCategory",
	"BinaryRelationshipRule"."TargetCategory",
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

CREATE VIEW "SiteDirectory"."MultiRelationshipRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "MultiRelationshipRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"MultiRelationshipRule"."RelationshipCategory",
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

CREATE VIEW "SiteDirectory"."DecompositionRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "DecompositionRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"DecompositionRule"."ContainingCategory",
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

CREATE VIEW "SiteDirectory"."ParameterizedCategoryRule_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Rule"."ValueTypeDictionary" || "ParameterizedCategoryRule"."ValueTypeDictionary" AS "ValueTypeSet",
	"Rule"."Container",
	NULL::bigint AS "Sequence",
	"ParameterizedCategoryRule"."Category",
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

CREATE VIEW "SiteDirectory"."Constant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "Constant"."ValueTypeDictionary" AS "ValueTypeSet",
	"Constant"."Container",
	NULL::bigint AS "Sequence",
	"Constant"."ParameterType",
	"Constant"."Scale",
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

CREATE VIEW "SiteDirectory"."EngineeringModelSetup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "EngineeringModelSetup"."ValueTypeDictionary" AS "ValueTypeSet",
	"EngineeringModelSetup"."Container",
	NULL::bigint AS "Sequence",
	COALESCE("DefinedThing_Alias"."Alias",'{}'::text[]) AS "Alias",
	COALESCE("DefinedThing_Definition"."Definition",'{}'::text[]) AS "Definition",
	COALESCE("DefinedThing_HyperLink"."HyperLink",'{}'::text[]) AS "HyperLink",
	COALESCE("EngineeringModelSetup_Participant"."Participant",'{}'::text[]) AS "Participant",
	COALESCE("EngineeringModelSetup_RequiredRdl"."RequiredRdl",'{}'::text[]) AS "RequiredRdl",
	COALESCE("EngineeringModelSetup_IterationSetup"."IterationSetup",'{}'::text[]) AS "IterationSetup",
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
   GROUP BY "IterationSetup"."Container") AS "EngineeringModelSetup_IterationSetup" USING ("Iid");

CREATE VIEW "SiteDirectory"."Participant_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "Participant"."ValueTypeDictionary" AS "ValueTypeSet",
	"Participant"."Container",
	NULL::bigint AS "Sequence",
	"Participant"."Person",
	"Participant"."Role",
	"Participant"."SelectedDomain",
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
   GROUP BY "Participant") AS "Participant_Domain" USING ("Iid");

CREATE VIEW "SiteDirectory"."ModelReferenceDataLibrary_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "ReferenceDataLibrary"."ValueTypeDictionary" || "ModelReferenceDataLibrary"."ValueTypeDictionary" AS "ValueTypeSet",
	"ModelReferenceDataLibrary"."Container",
	NULL::bigint AS "Sequence",
	"ReferenceDataLibrary"."RequiredRdl",
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

CREATE VIEW "SiteDirectory"."IterationSetup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "IterationSetup"."ValueTypeDictionary" AS "ValueTypeSet",
	"IterationSetup"."Container",
	NULL::bigint AS "Sequence",
	"IterationSetup"."SourceIterationSetup",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."PersonRole_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "PersonRole"."ValueTypeDictionary" AS "ValueTypeSet",
	"PersonRole"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."PersonPermission_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "PersonPermission"."ValueTypeDictionary" AS "ValueTypeSet",
	"PersonPermission"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."SiteLogEntry_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "SiteLogEntry"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteLogEntry"."Container",
	NULL::bigint AS "Sequence",
	"SiteLogEntry"."Author",
	COALESCE("Thing_ExcludedPerson"."ExcludedPerson",'{}'::text[]) AS "ExcludedPerson",
	COALESCE("Thing_ExcludedDomain"."ExcludedDomain",'{}'::text[]) AS "ExcludedDomain",
	COALESCE("SiteLogEntry_Category"."Category",'{}'::text[]) AS "Category",
	COALESCE("SiteLogEntry_AffectedItemIid"."AffectedItemIid",'{}'::text[]) AS "AffectedItemIid"
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
   GROUP BY "SiteLogEntry") AS "SiteLogEntry_AffectedItemIid" USING ("Iid");

CREATE VIEW "SiteDirectory"."DomainOfExpertiseGroup_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "DomainOfExpertiseGroup"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainOfExpertiseGroup"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."DomainOfExpertise_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "DefinedThing"."ValueTypeDictionary" || "DomainOfExpertise"."ValueTypeDictionary" AS "ValueTypeSet",
	"DomainOfExpertise"."Container",
	NULL::bigint AS "Sequence",
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

CREATE VIEW "SiteDirectory"."NaturalLanguage_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "NaturalLanguage"."ValueTypeDictionary" AS "ValueTypeSet",
	"NaturalLanguage"."Container",
	NULL::bigint AS "Sequence",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."GenericAnnotation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" AS "ValueTypeSet",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."SiteDirectoryDataAnnotation_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "SiteDirectoryDataAnnotation"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectoryDataAnnotation"."Container",
	NULL::bigint AS "Sequence",
	"SiteDirectoryDataAnnotation"."Author",
	"SiteDirectoryDataAnnotation"."PrimaryAnnotatedThing",
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
  LEFT JOIN (SELECT "SiteDirectoryThingReference"."Container" AS "Iid", array_agg("SiteDirectoryThingReference"."Iid"::text) AS "RelatedThing"
   FROM "SiteDirectory"."SiteDirectoryThingReference_Data"() AS "SiteDirectoryThingReference"
   JOIN "SiteDirectory"."SiteDirectoryDataAnnotation_Data"() AS "SiteDirectoryDataAnnotation" ON "SiteDirectoryThingReference"."Container" = "SiteDirectoryDataAnnotation"."Iid"
   GROUP BY "SiteDirectoryThingReference"."Container") AS "SiteDirectoryDataAnnotation_RelatedThing" USING ("Iid")
  LEFT JOIN (SELECT "SiteDirectoryDataDiscussionItem"."Container" AS "Iid", array_agg("SiteDirectoryDataDiscussionItem"."Iid"::text) AS "Discussion"
   FROM "SiteDirectory"."SiteDirectoryDataDiscussionItem_Data"() AS "SiteDirectoryDataDiscussionItem"
   JOIN "SiteDirectory"."SiteDirectoryDataAnnotation_Data"() AS "SiteDirectoryDataAnnotation" ON "SiteDirectoryDataDiscussionItem"."Container" = "SiteDirectoryDataAnnotation"."Iid"
   GROUP BY "SiteDirectoryDataDiscussionItem"."Container") AS "SiteDirectoryDataAnnotation_Discussion" USING ("Iid");

CREATE VIEW "SiteDirectory"."ThingReference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ThingReference"."ValueTypeDictionary" AS "ValueTypeSet",
	"ThingReference"."ReferencedThing",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."SiteDirectoryThingReference_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "ThingReference"."ValueTypeDictionary" || "SiteDirectoryThingReference"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectoryThingReference"."Container",
	NULL::bigint AS "Sequence",
	"ThingReference"."ReferencedThing",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."DiscussionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "DiscussionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"DiscussionItem"."ReplyTo",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");

CREATE VIEW "SiteDirectory"."SiteDirectoryDataDiscussionItem_View" AS
 SELECT "Thing"."Iid", "Thing"."ValueTypeDictionary" || "GenericAnnotation"."ValueTypeDictionary" || "DiscussionItem"."ValueTypeDictionary" || "SiteDirectoryDataDiscussionItem"."ValueTypeDictionary" AS "ValueTypeSet",
	"SiteDirectoryDataDiscussionItem"."Container",
	NULL::bigint AS "Sequence",
	"DiscussionItem"."ReplyTo",
	"SiteDirectoryDataDiscussionItem"."Author",
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
   GROUP BY "Thing") AS "Thing_ExcludedDomain" USING ("Iid");
