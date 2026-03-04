BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250220110114_AddProductsTable3')
BEGIN
    INSERT INTO LoginHistory (date, UserName, ip,recaptchascore,response,jwttokenexpirydate,online) VALUES (getdate(), 'JohnDoe1','123','0','Ok' ,getdate(),1)
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250220110114_AddProductsTable3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250220110114_AddProductsTable3', N'7.0.0');
END;
GO

COMMIT;
GO

