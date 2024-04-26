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

CREATE EXTENSION IF NOT EXISTS hstore;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE SCHEMA "EngineeringModel_REPLACE";

-- RevisionRegistry
CREATE SEQUENCE "EngineeringModel_REPLACE"."Revision" MINVALUE 1 START 1;
CREATE TABLE "EngineeringModel_REPLACE"."RevisionRegistry"
(
  "Revision" integer NOT NULL,
  "Instant" timestamp NOT NULL,
  "Actor" uuid
);
ALTER TABLE "EngineeringModel_REPLACE"."RevisionRegistry" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RevisionRegistry" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."RevisionRegistry" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."RevisionRegistry" SET (autovacuum_analyze_threshold = 2500);

-- EngineeringModel get_current_revision
CREATE OR REPLACE FUNCTION "EngineeringModel_REPLACE".get_current_revision() RETURNS INTEGER 
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
  SELECT "Revision" INTO revision FROM "EngineeringModel_REPLACE"."RevisionRegistry" WHERE "Instant" = transaction_time;
  
  IF(revision IS NULL) THEN
  
    -- no revision registry entry for this transaction; increase revision number
    SELECT nextval('"EngineeringModel_REPLACE"."Revision"') INTO revision;
    EXECUTE 'INSERT INTO "EngineeringModel_REPLACE"."RevisionRegistry" ("Revision", "Instant", "Actor") VALUES($1, $2, $3);' USING revision, transaction_time, actor_id;
  
    -- make sure to log the updated state of top container updates (even if audit logging is temporarily turned off)
    audit_enabled := "SiteDirectory".get_audit_enabled();
    IF (NOT audit_enabled) THEN
      -- enabled audit logging
      EXECUTE 'UPDATE transaction_info SET audit_enabled = true;';
    END IF;

    -- update the revision number and last modified on properties of the top container
    EXECUTE 'UPDATE "EngineeringModel_REPLACE"."TopContainer" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"LastModifiedOn" => "' || transaction_time || '"'';';
    EXECUTE 'UPDATE "EngineeringModel_REPLACE"."Thing" SET "ValueTypeDictionary" = "ValueTypeDictionary" || ''"RevisionNumber" => "' || revision || '"'' WHERE "Iid" = ANY(SELECT "Iid" FROM "EngineeringModel_REPLACE"."TopContainer");';
  
    IF (NOT audit_enabled) THEN
      -- turn off auditing again for remainder of transaction
      EXECUTE 'UPDATE transaction_info SET audit_enabled = false;';
    END IF;

  END IF;
  
  -- return the current revision number
  RETURN revision;
END;
$$;

-- IterationRevisionLog
CREATE TABLE "EngineeringModel_REPLACE"."IterationRevisionLog"
(
  "IterationIid" uuid NOT NULL,
  "FromRevision" integer NOT NULL DEFAULT "EngineeringModel_REPLACE".get_current_revision(),
  "ToRevision" integer
);
ALTER TABLE "EngineeringModel_REPLACE"."IterationRevisionLog" SET (autovacuum_vacuum_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."IterationRevisionLog" SET (autovacuum_vacuum_threshold = 2500);
ALTER TABLE "EngineeringModel_REPLACE"."IterationRevisionLog" SET (autovacuum_analyze_scale_factor = 0.0);
ALTER TABLE "EngineeringModel_REPLACE"."IterationRevisionLog" SET (autovacuum_analyze_threshold = 2500);

CREATE SCHEMA "Iteration_REPLACE";
