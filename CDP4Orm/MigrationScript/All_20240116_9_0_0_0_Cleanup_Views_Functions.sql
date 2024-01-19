DO $$ 
DECLARE 
    drop_statement text;
BEGIN
    -- Drop views
    FOR drop_statement IN (
        SELECT format('DROP VIEW IF EXISTS %I.%I CASCADE;', v.schemaname, v.viewname) AS drop_statement
        FROM pg_views v
        WHERE 
            v.schemaname IN ('SchemaName_Replace')
    )
    LOOP
        EXECUTE drop_statement;
    END LOOP;

    -- Drop functions
    FOR drop_statement IN (
        SELECT format('DROP FUNCTION IF EXISTS %I.%I() CASCADE;', r.specific_schema, r.routine_name) AS drop_statement
        FROM information_schema.routines r
        WHERE 
            r.specific_schema IN ('SchemaName_Replace') AND
            r.routine_name LIKE '%_Data'
    )
    LOOP
        EXECUTE drop_statement;
    END LOOP;
END $$;