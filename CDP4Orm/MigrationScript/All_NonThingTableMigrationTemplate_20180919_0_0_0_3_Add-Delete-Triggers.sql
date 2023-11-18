DROP TRIGGER IF EXISTS TABLE_REPLACE_Thing_Delete_Trigger on "SchemaName_Replace"."TABLE_REPLACE";
CREATE TRIGGER TABLE_REPLACE_Thing_Delete_Trigger
  AFTER DELETE ON "SchemaName_Replace"."TABLE_REPLACE"
  FOR EACH ROW 
  EXECUTE PROCEDURE "SchemaName_Replace".delete_thing_record();