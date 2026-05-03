-- SQL Script to apply migration
-- Add AvatarPath column to AspNetUsers table

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'AvatarPath')
BEGIN
    ALTER TABLE AspNetUsers ADD AvatarPath nvarchar(max) NULL;
    PRINT 'Column AvatarPath added successfully to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'Column AvatarPath already exists in AspNetUsers table';
END
