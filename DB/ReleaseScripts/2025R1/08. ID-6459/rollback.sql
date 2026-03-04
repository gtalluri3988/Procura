BEGIN TRANSACTION;
GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250220113532_Addcolumn_data_History')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LoginHistory]') AND [c].[name] = N'Data');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [LoginHistory] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [LoginHistory] DROP COLUMN [Data];
END;
GO

IF EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250220113532_Addcolumn_data_History')
BEGIN
    DELETE FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250220113532_Addcolumn_data_History';
END;
GO

COMMIT;
GO

