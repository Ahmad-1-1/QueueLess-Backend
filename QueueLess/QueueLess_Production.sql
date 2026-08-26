IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [BusinessCategories] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_BusinessCategories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [MobileNumber] nvarchar(20) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Businesses] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Businesses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Businesses_BusinessCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [BusinessCategories] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PlatformAdmins] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_PlatformAdmins] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlatformAdmins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Staff] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [ServiceId] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Staff] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Staff_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Services] (
    [Id] uniqueidentifier NOT NULL,
    [BusinessId] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [AssignedStaffId] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    [AvgServiceTimeMinutes] int NOT NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Services_Businesses_BusinessId] FOREIGN KEY ([BusinessId]) REFERENCES [Businesses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Services_Staff_AssignedStaffId] FOREIGN KEY ([AssignedStaffId]) REFERENCES [Staff] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Tickets] (
    [Id] uniqueidentifier NOT NULL,
    [ServiceId] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [QueueNumber] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [PositionSnapshot] int NOT NULL,
    [JoinedAt] datetime2 NOT NULL,
    [ServedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    CONSTRAINT [PK_Tickets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tickets_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Tickets_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [WorkingHours] (
    [Id] uniqueidentifier NOT NULL,
    [ServiceId] uniqueidentifier NOT NULL,
    [DayOfWeek] int NOT NULL,
    [OpenTime] time NOT NULL,
    [CloseTime] time NOT NULL,
    CONSTRAINT [PK_WorkingHours] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WorkingHours_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Notifications] (
    [Id] uniqueidentifier NOT NULL,
    [TicketId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Businesses_CategoryId] ON [Businesses] ([CategoryId]);
GO

CREATE INDEX [IX_Notifications_TicketId] ON [Notifications] ([TicketId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_PlatformAdmins_UserId] ON [PlatformAdmins] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Services_AssignedStaffId] ON [Services] ([AssignedStaffId]) WHERE [AssignedStaffId] IS NOT NULL;
GO

CREATE INDEX [IX_Services_BusinessId] ON [Services] ([BusinessId]);
GO

CREATE UNIQUE INDEX [IX_Staff_UserId] ON [Staff] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Tickets_CustomerId] ON [Tickets] ([CustomerId]) WHERE [Status] IN ('Waiting', 'Serving');
GO

CREATE INDEX [IX_Tickets_ServiceId] ON [Tickets] ([ServiceId]);
GO

CREATE UNIQUE INDEX [IX_Users_MobileNumber] ON [Users] ([MobileNumber]);
GO

CREATE INDEX [IX_WorkingHours_ServiceId] ON [WorkingHours] ([ServiceId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260811131607_InitialMigration', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Users] ADD [Email] nvarchar(150) NULL;
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [Email] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260811133252_AddEmailToUser', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Users_Email] ON [Users];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Email');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Users] ALTER COLUMN [Email] nvarchar(256) NOT NULL;
GO

CREATE TABLE [OtpRequests] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [OtpCode] nvarchar(max) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_OtpRequests] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260818123128_MakeEmailRequiredAndAddOtpRequest', N'8.0.8');
GO

COMMIT;
GO

