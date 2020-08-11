DO $$DECLARE r record;
BEGIN
    FOR r IN 
        (SELECT 'ALTER TABLE "'|| schemaname || '"."' || tablename ||'" SET (autovacuum_vacuum_scale_factor = 0.0);' as a FROM pg_tables WHERE NOT schemaname IN ('pg_catalog', 'information_schema') ORDER BY schemaname, tablename)
		union all
		(SELECT 'ALTER TABLE "'|| schemaname || '"."' || tablename ||'" SET (autovacuum_vacuum_threshold = 2500);' as a FROM pg_tables WHERE NOT schemaname IN ('pg_catalog', 'information_schema') ORDER BY schemaname, tablename)
		union all
		(SELECT 'ALTER TABLE "'|| schemaname || '"."' || tablename ||'" SET (autovacuum_analyze_scale_factor = 0.0);' as a FROM pg_tables WHERE NOT schemaname IN ('pg_catalog', 'information_schema') ORDER BY schemaname, tablename)
		union all
		(SELECT 'ALTER TABLE "'|| schemaname || '"."' || tablename ||'" SET (autovacuum_analyze_threshold = 2500);' as a FROM pg_tables WHERE NOT schemaname IN ('pg_catalog', 'information_schema') ORDER BY schemaname, tablename)
		LOOP
        EXECUTE r.a;
    END LOOP;
END$$;
