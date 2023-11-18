-- End validity of all data in a given partition
-- This is used for new iteration creation
-- Ending the validity is done by doing an update on all the tables of the schema

CREATE OR REPLACE FUNCTION "SiteDirectory".end_all_current_data_validity(partition_name text)
RETURNS void LANGUAGE plpgsql
AS $BODY$ 
DECLARE 
  trigger_name text;
  current_table text;
  audit_offset smallint;
  audit_tables CURSOR FOR
    SELECT *
    FROM information_schema.tables
    WHERE table_schema = partition_name
    AND table_name like '%\_Audit';
BEGIN
   audit_offset = 5;
   FOR audit_table_name IN audit_tables LOOP
     current_table = substr(audit_table_name.table_name, 0, length(audit_table_name.table_name) - audit_offset);

    -- disable revision trigger for association table (update case of revision_management is not adapted to an update of these tables)
    IF current_table LIKE '%\_%' THEN
        trigger_name = LOWER(current_table) || '_apply_revision';
        EXECUTE format('ALTER TABLE %I.%I DISABLE TRIGGER %I;', partition_name, current_table, trigger_name);
    END IF;
     
    EXECUTE format('UPDATE %I.%I SET "ValidTo" = $1;', partition_name, current_table) USING 'infinity'::timestamp without time zone;

    IF current_table LIKE '%\_%' THEN
        trigger_name = LOWER(current_table) || '_apply_revision';
        EXECUTE format('ALTER TABLE %I.%I ENABLE TRIGGER %I;', partition_name, current_table, trigger_name);
    END IF;
    END LOOP;
END $BODY$;

-- populate all the "current thing tables" with data from the audit tables for a given partition and instant
CREATE OR REPLACE FUNCTION "SiteDirectory".insert_data_from_audit(partition_name text, instant timestamp without time zone)
RETURNS void LANGUAGE plpgsql
AS $BODY$ 
DECLARE 
  sql_column text;
  current_table text;
  current_audit_table text;
  audit_offset smallint;
  audit_tables CURSOR FOR
    SELECT *
    FROM information_schema.tables
    WHERE table_schema = partition_name
    AND table_name like '%\_Audit';
BEGIN
    audit_offset = 5;

    SET CONSTRAINTS ALL DEFERRED;
    FOR audit_table_name IN audit_tables LOOP
        current_table = substr(audit_table_name.table_name, 0, length(audit_table_name.table_name) - audit_offset);
        current_audit_table = audit_table_name.table_name;
sql_column = '';

        SELECT array_to_string(ARRAY(SELECT '"' || c.column_name || '"'
            FROM information_schema.columns As c
                WHERE table_name = current_audit_table
                AND table_schema = partition_name 
                AND  c.column_name NOT IN('ValidFrom', 'ValidTo', 'Action', 'Actor')
            ), ',') INTO sql_column;

        EXECUTE format('INSERT INTO %I.%I (%s) SELECT %s FROM %I.%I WHERE "Action" <> $2 AND "ValidFrom" < $1 AND "ValidTo" >= $1', partition_name, current_table, sql_column, sql_column, partition_name, current_audit_table) USING instant, 'I';
    END LOOP;
END $BODY$;