DO $$DECLARE r record;
BEGIN
    FOR r IN 
        (SELECT 'ALTER TABLE "SchemaName_Replace"."' || tablename ||'" SET (autovacuum_vacuum_scale_factor = 0.0);' as a FROM pg_tables WHERE schemaname='SchemaName_Replace' ORDER BY tablename)
		union all
		(SELECT 'ALTER TABLE "SchemaName_Replace"."' || tablename ||'" SET (autovacuum_vacuum_threshold = 2500);' as a FROM pg_tables WHERE schemaname='SchemaName_Replace' ORDER BY tablename)
		union all
		(SELECT 'ALTER TABLE "SchemaName_Replace"."' || tablename ||'" SET (autovacuum_analyze_scale_factor = 0.0);' as a FROM pg_tables WHERE schemaname='SchemaName_Replace' ORDER BY tablename)
		union all
		(SELECT 'ALTER TABLE "SchemaName_Replace"."' || tablename ||'" SET (autovacuum_analyze_threshold = 2500);' as a FROM pg_tables WHERE schemaname='SchemaName_Replace' ORDER BY tablename)
		LOOP
        EXECUTE r.a;
    END LOOP;
END$$;
