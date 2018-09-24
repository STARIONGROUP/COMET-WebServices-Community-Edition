CREATE TABLE IF NOT EXISTS "SiteDirectory"."MigrationManagement" (
    "version" text CONSTRAINT migrationPK PRIMARY KEY,
    "name" text,
    "date" text,
    "scope" text,
    "resource_name" text
);