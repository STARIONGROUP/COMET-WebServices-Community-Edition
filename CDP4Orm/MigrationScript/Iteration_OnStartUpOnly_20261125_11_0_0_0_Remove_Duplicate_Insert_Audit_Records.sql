-- Creating an iteration from an older iteration replayed the iteration data and logged a second open insert ('I')
-- audit record for every replayed thing (STARIONGROUP/COMET-IME-Community-Edition#1518). The Actor join of the DAO
-- read queries then returns a thing once per distinct Actor. Keep only the oldest open insert audit record (the real
-- creator) of every Iid. Insert audit records are not used by time-travel reads nor by insert_data_from_audit.
DO $$
DECLARE
  audit_table record;
BEGIN
  FOR audit_table IN
    SELECT table_name FROM information_schema.columns
    WHERE table_schema = 'SchemaName_Replace' AND table_name LIKE '%\_Audit' AND column_name = 'Iid'
  LOOP
    EXECUTE format('DELETE FROM %I.%I AS duplicate WHERE duplicate."Action" = ''I'' AND duplicate."ValidTo" = ''infinity''
      AND EXISTS (SELECT 1 FROM %I.%I AS original WHERE original."Iid" = duplicate."Iid" AND original."Action" = ''I''
      AND original."ValidTo" = ''infinity'' AND original."ValidFrom" < duplicate."ValidFrom")',
      'SchemaName_Replace', audit_table.table_name, 'SchemaName_Replace', audit_table.table_name);
  END LOOP;
END $$;
