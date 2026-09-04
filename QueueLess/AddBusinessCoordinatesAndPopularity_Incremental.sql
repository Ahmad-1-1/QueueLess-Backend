BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Businesses]') AND [c].[name] = N'Location');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Businesses] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Businesses] DROP COLUMN [Location];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Businesses]') AND [c].[name] = N'Tag');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Businesses] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Businesses] DROP COLUMN [Tag];
GO

ALTER TABLE [Businesses] ADD [Latitude] float(9) NULL;
GO

ALTER TABLE [Businesses] ADD [Longitude] float(9) NULL;
GO

ALTER TABLE [Businesses] ADD [PopularityScore] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260901143433_AddBusinessCoordinatesAndPopularity', N'8.0.8');
GO

COMMIT;
GO

