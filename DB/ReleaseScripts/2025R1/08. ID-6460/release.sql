BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250220113532_Addcolumn_data_History')
BEGIN
    ALTER TABLE [LoginHistory] ADD [Data] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250220113532_Addcolumn_data_History')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250220113532_Addcolumn_data_History', N'7.0.0');
END;
GO

COMMIT;
GO

