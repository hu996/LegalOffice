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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Clients] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [WhatsAppPhone] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [NationalId] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [ClientType] nvarchar(max) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Clients] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Lawyers] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [Email] nvarchar(450) NULL,
        [JobTitle] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [UserId] nvarchar(max) NULL,
        CONSTRAINT [PK_Lawyers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Lookups] (
        [Id] int NOT NULL IDENTITY,
        [Type] nvarchar(450) NOT NULL,
        [NameAr] nvarchar(450) NOT NULL,
        [NameEn] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Lookups] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [MessageLogs] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NULL,
        [ClientId] int NOT NULL,
        [Channel] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [MessageText] nvarchar(max) NOT NULL,
        [IsSent] bit NOT NULL,
        [ProviderResponse] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_MessageLogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [MessageTemplates] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Channel] nvarchar(max) NOT NULL,
        [Body] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_MessageTemplates] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [LawyerId] int NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUsers_Lawyers_LawyerId] FOREIGN KEY ([LawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Cases] (
        [Id] int NOT NULL IDENTITY,
        [CaseNumber] nvarchar(max) NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [ClientId] int NOT NULL,
        [CaseTypeId] int NOT NULL,
        [CaseStatusId] int NOT NULL,
        [CourtId] int NULL,
        [Circuit] nvarchar(max) NULL,
        [OpponentName] nvarchar(max) NULL,
        [OpponentLawyer] nvarchar(max) NULL,
        [StartDate] datetime2 NOT NULL,
        [ClosedDate] datetime2 NULL,
        [FeesAmount] decimal(18,2) NOT NULL,
        [LawyersCount] int NOT NULL,
        [Priority] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Cases] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cases_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cases_Lookups_CaseStatusId] FOREIGN KEY ([CaseStatusId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cases_Lookups_CaseTypeId] FOREIGN KEY ([CaseTypeId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cases_Lookups_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [CaseDocuments] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [DocumentTypeId] int NOT NULL,
        [FileName] nvarchar(max) NOT NULL,
        [FilePath] nvarchar(max) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CaseDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseDocuments_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseDocuments_Lookups_DocumentTypeId] FOREIGN KEY ([DocumentTypeId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [CaseHearings] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [HearingDate] datetime2 NOT NULL,
        [HearingStatusId] int NOT NULL,
        [CourtDecision] nvarchar(max) NULL,
        [Notes] nvarchar(max) NULL,
        [NextRequirements] nvarchar(max) NULL,
        [NextHearingDate] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CaseHearings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseHearings_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseHearings_Lookups_HearingStatusId] FOREIGN KEY ([HearingStatusId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [CaseLawyers] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [LawyerId] int NOT NULL,
        [IsMainLawyer] bit NOT NULL,
        [RoleInCase] nvarchar(max) NULL,
        [AccessLevel] int NOT NULL,
        CONSTRAINT [PK_CaseLawyers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseLawyers_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseLawyers_Lawyers_LawyerId] FOREIGN KEY ([LawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [CaseTimelines] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [EventType] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CaseTimelines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseTimelines_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Expenses] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [ExpenseTypeId] int NULL,
        [Amount] decimal(18,2) NOT NULL,
        [ExpenseDate] datetime2 NOT NULL,
        [Notes] nvarchar(max) NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Expenses_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Expenses_Lookups_ExpenseTypeId] FOREIGN KEY ([ExpenseTypeId]) REFERENCES [Lookups] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [PaymentStatusId] int NOT NULL,
        [PaymentMethodId] int NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [ReceivedByUserId] nvarchar(450) NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_AspNetUsers_ReceivedByUserId] FOREIGN KEY ([ReceivedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Payments_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Payments_Lookups_PaymentMethodId] FOREIGN KEY ([PaymentMethodId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Lookups_PaymentStatusId] FOREIGN KEY ([PaymentStatusId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetUsers_LawyerId] ON [AspNetUsers] ([LawyerId]) WHERE [LawyerId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_CaseDocuments_CaseId] ON [CaseDocuments] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_CaseDocuments_DocumentTypeId] ON [CaseDocuments] ([DocumentTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_CaseHearings_CaseId] ON [CaseHearings] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_CaseHearings_HearingStatusId] ON [CaseHearings] ([HearingStatusId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CaseLawyers_CaseId_LawyerId] ON [CaseLawyers] ([CaseId], [LawyerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_CaseLawyers_LawyerId] ON [CaseLawyers] ([LawyerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Cases_CaseStatusId] ON [Cases] ([CaseStatusId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Cases_CaseTypeId] ON [Cases] ([CaseTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Cases_ClientId] ON [Cases] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Cases_CourtId] ON [Cases] ([CourtId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_CaseTimelines_CaseId] ON [CaseTimelines] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Expenses_CaseId] ON [Expenses] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Expenses_ExpenseTypeId] ON [Expenses] ([ExpenseTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Lawyers_Email] ON [Lawyers] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Lookups_Type_NameAr] ON [Lookups] ([Type], [NameAr]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Payments_CaseId] ON [Payments] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Payments_PaymentMethodId] ON [Payments] ([PaymentMethodId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Payments_PaymentStatusId] ON [Payments] ([PaymentStatusId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    CREATE INDEX [IX_Payments_ReceivedByUserId] ON [Payments] ([ReceivedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624002053_InitialSqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624002053_InitialSqlServer', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    EXEC sp_rename N'[CaseLawyers].[AccessLevel]', N'AccessLevelId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clients]') AND [c].[name] = N'NationalId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Clients] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [Clients] SET [NationalId] = N'''' WHERE [NationalId] IS NULL');
    ALTER TABLE [Clients] ALTER COLUMN [NationalId] nvarchar(450) NOT NULL;
    ALTER TABLE [Clients] ADD DEFAULT N'' FOR [NationalId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN

    UPDATE Clients
    SET NationalId = CAST(10000000000000 + Id AS nvarchar(14))
    WHERE NationalId = '';

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cases]') AND [c].[name] = N'CaseNumber');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Cases] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Cases] ALTER COLUMN [CaseNumber] nvarchar(450) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    ALTER TABLE [Cases] ADD [PriorityId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    ALTER TABLE [CaseDocuments] ADD [Notes] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE TABLE [LawyerSpecialties] (
        [Id] int NOT NULL IDENTITY,
        [LawyerId] int NOT NULL,
        [CaseTypeId] int NOT NULL,
        CONSTRAINT [PK_LawyerSpecialties] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LawyerSpecialties_Lawyers_LawyerId] FOREIGN KEY ([LawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LawyerSpecialties_Lookups_CaseTypeId] FOREIGN KEY ([CaseTypeId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'View')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CaseAccessLevel', N'عرض فقط', 'View', 1);
    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Edit')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CaseAccessLevel', N'تعديل', 'Edit', 1);
    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Manage')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CaseAccessLevel', N'إدارة كاملة', 'Manage', 1);
    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Low')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'منخفضة', 'Low', 1);
    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Medium')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'متوسطة', 'Medium', 1);
    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'High')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'عالية', 'High', 1);
    IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Critical')
        INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'حرجة', 'Critical', 1);

    UPDATE cl
    SET cl.AccessLevelId =
        CASE cl.AccessLevelId
            WHEN 1 THEN (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'View' ORDER BY Id)
            WHEN 2 THEN (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Edit' ORDER BY Id)
            WHEN 3 THEN (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Manage' ORDER BY Id)
            ELSE (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'View' ORDER BY Id)
        END
    FROM CaseLawyers cl;

    UPDATE c
    SET c.PriorityId = (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Medium' ORDER BY Id)
    FROM Cases c;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clients_NationalId] ON [Clients] ([NationalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cases_CaseNumber] ON [Cases] ([CaseNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE INDEX [IX_Cases_PriorityId] ON [Cases] ([PriorityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE INDEX [IX_CaseLawyers_AccessLevelId] ON [CaseLawyers] ([AccessLevelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE INDEX [IX_LawyerSpecialties_CaseTypeId] ON [LawyerSpecialties] ([CaseTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LawyerSpecialties_LawyerId_CaseTypeId] ON [LawyerSpecialties] ([LawyerId], [CaseTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    ALTER TABLE [CaseLawyers] ADD CONSTRAINT [FK_CaseLawyers_Lookups_AccessLevelId] FOREIGN KEY ([AccessLevelId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    ALTER TABLE [Cases] ADD CONSTRAINT [FK_Cases_Lookups_PriorityId] FOREIGN KEY ([PriorityId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cases]') AND [c].[name] = N'Priority');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Cases] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Cases] DROP COLUMN [Priority];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624010652_ValidationAndSpecialties'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624010652_ValidationAndSpecialties', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    DROP INDEX [IX_Lookups_Type_NameAr] ON [Lookups];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Lookups]') AND [c].[name] = N'Type');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Lookups] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Lookups] ALTER COLUMN [Type] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Lookups]') AND [c].[name] = N'NameEn');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Lookups] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Lookups] ALTER COLUMN [NameEn] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Lookups]') AND [c].[name] = N'NameAr');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Lookups] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Lookups] ALTER COLUMN [NameAr] nvarchar(200) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    ALTER TABLE [Lookups] ADD [LookupTypeId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    CREATE TABLE [LookupTypes] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(200) NOT NULL,
        [NameEn] nvarchar(200) NULL,
        [IsActive] bit NOT NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_LookupTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN

    INSERT INTO LookupTypes (Code, NameAr, NameEn, IsActive, SortOrder)
    SELECT DISTINCT l.[Type], l.[Type], l.[Type], CAST(1 AS bit), 0
    FROM Lookups l
    WHERE NOT EXISTS (
        SELECT 1
        FROM LookupTypes lt
        WHERE lt.Code = l.[Type]
    )
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN

    UPDATE l
    SET l.LookupTypeId = lt.Id
    FROM Lookups l
    INNER JOIN LookupTypes lt ON lt.Code = l.[Type]

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Lookups_LookupTypeId_NameAr] ON [Lookups] ([LookupTypeId], [NameAr]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LookupTypes_Code] ON [LookupTypes] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    ALTER TABLE [Lookups] ADD CONSTRAINT [FK_Lookups_LookupTypes_LookupTypeId] FOREIGN KEY ([LookupTypeId]) REFERENCES [LookupTypes] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624014611_LookupTypesAndPaging'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624014611_LookupTypesAndPaging', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    CREATE TABLE [SystemPermissions] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(120) NOT NULL,
        [NameAr] nvarchar(200) NOT NULL,
        [NameEn] nvarchar(200) NULL,
        [Controller] nvarchar(120) NOT NULL,
        [Action] nvarchar(120) NOT NULL,
        [MenuGroup] nvarchar(80) NULL,
        [SortOrder] int NOT NULL,
        [IsMenuItem] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_SystemPermissions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [Id] int NOT NULL IDENTITY,
        [RoleName] nvarchar(256) NOT NULL,
        [SystemPermissionId] int NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RolePermissions_SystemPermissions_SystemPermissionId] FOREIGN KEY ([SystemPermissionId]) REFERENCES [SystemPermissions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RolePermissions_RoleName_SystemPermissionId] ON [RolePermissions] ([RoleName], [SystemPermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_SystemPermissionId] ON [RolePermissions] ([SystemPermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemPermissions_Code] ON [SystemPermissions] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemPermissions_Controller_Action] ON [SystemPermissions] ([Controller], [Action]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624020322_DynamicPermissions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624020322_DynamicPermissions', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624024000_Notifications'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [LawyerId] int NOT NULL,
        [CaseId] int NULL,
        [Title] nvarchar(200) NOT NULL,
        [Body] nvarchar(500) NOT NULL,
        [TargetUrl] nvarchar(500) NOT NULL,
        [Type] nvarchar(80) NOT NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ReadAt] datetime2 NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Notifications_Lawyers_LawyerId] FOREIGN KEY ([LawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624024000_Notifications'
)
BEGIN
    CREATE INDEX [IX_Notifications_CaseId] ON [Notifications] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624024000_Notifications'
)
BEGIN
    CREATE INDEX [IX_Notifications_LawyerId_IsRead_CreatedAt] ON [Notifications] ([LawyerId], [IsRead], [CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624024000_Notifications'
)
BEGIN
    CREATE INDEX [IX_Notifications_LawyerId] ON [Notifications] ([LawyerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624024000_Notifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624024000_Notifications', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [DepartmentId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [UserTypeId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_DepartmentId] ON [AspNetUsers] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_UserTypeId] ON [AspNetUsers] ([UserTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Lookups_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Lookups] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Lookups_UserTypeId] FOREIGN KEY ([UserTypeId]) REFERENCES [Lookups] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624030000_UserAdministration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624030000_UserAdministration', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Lawyers]') AND [c].[name] = N'Phone');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Lawyers] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Lawyers] ALTER COLUMN [Phone] nvarchar(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Lawyers]') AND [c].[name] = N'JobTitle');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Lawyers] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Lawyers] ALTER COLUMN [JobTitle] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Lawyers] ADD [BranchId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Lawyers] ADD [DepartmentId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD [ApprovedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD [ApprovedByUserId] nvarchar(450) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD [RejectionReason] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD [StatusLookupId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD [SubmittedByUserId] nvarchar(450) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clients]') AND [c].[name] = N'Notes');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Clients] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Clients] ALTER COLUMN [Notes] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clients]') AND [c].[name] = N'Address');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Clients] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Clients] ALTER COLUMN [Address] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Clients] ADD [BranchId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cases]') AND [c].[name] = N'OpponentName');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Cases] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Cases] ALTER COLUMN [OpponentName] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cases]') AND [c].[name] = N'OpponentLawyer');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Cases] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Cases] ALTER COLUMN [OpponentLawyer] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cases]') AND [c].[name] = N'Description');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Cases] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Cases] ALTER COLUMN [Description] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cases]') AND [c].[name] = N'Circuit');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Cases] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Cases] ALTER COLUMN [Circuit] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD [BranchId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD [DepartmentId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD [LastStageChangedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD [WorkflowStageLookupId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CaseHearings]') AND [c].[name] = N'Notes');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [CaseHearings] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [CaseHearings] ALTER COLUMN [Notes] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CaseHearings]') AND [c].[name] = N'NextRequirements');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [CaseHearings] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [CaseHearings] ALTER COLUMN [NextRequirements] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CaseHearings]') AND [c].[name] = N'CourtDecision');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [CaseHearings] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [CaseHearings] ALTER COLUMN [CourtDecision] nvarchar(1000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [BranchId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [UserName] nvarchar(max) NULL,
        [ActionType] nvarchar(max) NOT NULL,
        [EntityName] nvarchar(max) NOT NULL,
        [EntityId] nvarchar(max) NULL,
        [OldValues] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [UserAgent] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [Branches] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(200) NOT NULL,
        [Address] nvarchar(500) NULL,
        [Phone] nvarchar(20) NULL,
        [ManagerUserId] nvarchar(450) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Branches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Branches_AspNetUsers_ManagerUserId] FOREIGN KEY ([ManagerUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [CaseAssignmentHistories] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [LawyerId] int NOT NULL,
        [ActionType] nvarchar(max) NOT NULL,
        [ChangedByUserId] nvarchar(450) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CaseAssignmentHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseAssignmentHistories_AspNetUsers_ChangedByUserId] FOREIGN KEY ([ChangedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CaseAssignmentHistories_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseAssignmentHistories_Lawyers_LawyerId] FOREIGN KEY ([LawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [CaseInternalNotes] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [Note] nvarchar(2000) NOT NULL,
        [CreatedByUserId] nvarchar(450) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CaseInternalNotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseInternalNotes_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CaseInternalNotes_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [CaseStageHistories] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [FromStageLookupId] int NULL,
        [ToStageLookupId] int NOT NULL,
        [ChangedByUserId] nvarchar(450) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [ChangedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CaseStageHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CaseStageHistories_AspNetUsers_ChangedByUserId] FOREIGN KEY ([ChangedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CaseStageHistories_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CaseStageHistories_Lookups_FromStageLookupId] FOREIGN KEY ([FromStageLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_CaseStageHistories_Lookups_ToStageLookupId] FOREIGN KEY ([ToStageLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [ConflictChecks] (
        [Id] int NOT NULL IDENTITY,
        [ClientName] nvarchar(max) NOT NULL,
        [OpponentName] nvarchar(max) NOT NULL,
        [NationalId] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [CaseId] int NULL,
        [ResultStatusLookupId] int NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CheckedByUserId] nvarchar(450) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_ConflictChecks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConflictChecks_AspNetUsers_CheckedByUserId] FOREIGN KEY ([CheckedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConflictChecks_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_ConflictChecks_Lookups_ResultStatusLookupId] FOREIGN KEY ([ResultStatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [FeeAgreements] (
        [Id] int NOT NULL IDENTITY,
        [ClientId] int NOT NULL,
        [CaseId] int NULL,
        [FeeTypeLookupId] int NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [Notes] nvarchar(2000) NULL,
        CONSTRAINT [PK_FeeAgreements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeAgreements_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_FeeAgreements_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeeAgreements_Lookups_FeeTypeLookupId] FOREIGN KEY ([FeeTypeLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [Judgments] (
        [Id] int NOT NULL IDENTITY,
        [CaseId] int NOT NULL,
        [JudgmentDate] datetime2 NOT NULL,
        [CourtLevelLookupId] int NOT NULL,
        [JudgmentSummary] nvarchar(2000) NULL,
        [JudgmentAmount] decimal(18,2) NULL,
        [IsFinal] bit NOT NULL,
        [Notes] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Judgments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Judgments_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Judgments_Lookups_CourtLevelLookupId] FOREIGN KEY ([CourtLevelLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [LegalTasks] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [RelatedCaseId] int NULL,
        [AssignedToUserId] nvarchar(450) NOT NULL,
        [CreatedByUserId] nvarchar(450) NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [PriorityLookupId] int NOT NULL,
        [StatusLookupId] int NOT NULL,
        [TaskTypeLookupId] int NOT NULL,
        [CompletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_LegalTasks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LegalTasks_AspNetUsers_AssignedToUserId] FOREIGN KEY ([AssignedToUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalTasks_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalTasks_Cases_RelatedCaseId] FOREIGN KEY ([RelatedCaseId]) REFERENCES [Cases] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_LegalTasks_Lookups_PriorityLookupId] FOREIGN KEY ([PriorityLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalTasks_Lookups_StatusLookupId] FOREIGN KEY ([StatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalTasks_Lookups_TaskTypeLookupId] FOREIGN KEY ([TaskTypeLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [Meetings] (
        [Id] int NOT NULL IDENTITY,
        [Subject] nvarchar(200) NOT NULL,
        [ClientId] int NULL,
        [CaseId] int NULL,
        [AssignedUserId] nvarchar(450) NOT NULL,
        [MeetingDate] datetime2 NOT NULL,
        [MeetingResult] nvarchar(4000) NULL,
        [Notes] nvarchar(4000) NULL,
        [StatusLookupId] int NOT NULL,
        CONSTRAINT [PK_Meetings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Meetings_AspNetUsers_AssignedUserId] FOREIGN KEY ([AssignedUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Meetings_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Meetings_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Meetings_Lookups_StatusLookupId] FOREIGN KEY ([StatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [OfficeTreasuries] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [CurrentBalance] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_OfficeTreasuries] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [PowerOfAttorneys] (
        [Id] int NOT NULL IDENTITY,
        [PowerNumber] nvarchar(100) NOT NULL,
        [ClientId] int NOT NULL,
        [TypeLookupId] int NOT NULL,
        [IssueDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [RegistrationOffice] nvarchar(200) NULL,
        [Notes] nvarchar(2000) NULL,
        [FilePath] nvarchar(500) NULL,
        [StatusLookupId] int NOT NULL,
        [CaseId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PowerOfAttorneys] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PowerOfAttorneys_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_PowerOfAttorneys_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PowerOfAttorneys_Lookups_StatusLookupId] FOREIGN KEY ([StatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PowerOfAttorneys_Lookups_TypeLookupId] FOREIGN KEY ([TypeLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [Contracts] (
        [Id] int NOT NULL IDENTITY,
        [ContractNumber] nvarchar(100) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [ContractTypeLookupId] int NOT NULL,
        [ClientId] int NOT NULL,
        [AssignedLawyerId] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NULL,
        [ContractValue] decimal(18,2) NOT NULL,
        [StatusLookupId] int NOT NULL,
        [Notes] nvarchar(2000) NULL,
        [BranchId] int NULL,
        [DepartmentId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Contracts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contracts_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Contracts_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Contracts_Lawyers_AssignedLawyerId] FOREIGN KEY ([AssignedLawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Contracts_Lookups_ContractTypeLookupId] FOREIGN KEY ([ContractTypeLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Contracts_Lookups_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Lookups] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Contracts_Lookups_StatusLookupId] FOREIGN KEY ([StatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [LegalConsultations] (
        [Id] int NOT NULL IDENTITY,
        [ConsultationNumber] nvarchar(max) NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [ClientId] int NOT NULL,
        [AssignedLawyerId] int NOT NULL,
        [ConsultationTypeLookupId] int NOT NULL,
        [ConsultationStatusLookupId] int NOT NULL,
        [RequestDate] datetime2 NOT NULL,
        [ResponseDate] datetime2 NULL,
        [ConsultationFees] decimal(18,2) NOT NULL,
        [Subject] nvarchar(1000) NULL,
        [LegalOpinion] nvarchar(4000) NULL,
        [Notes] nvarchar(2000) NULL,
        [BranchId] int NULL,
        [DepartmentId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LegalConsultations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LegalConsultations_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_LegalConsultations_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalConsultations_Lawyers_AssignedLawyerId] FOREIGN KEY ([AssignedLawyerId]) REFERENCES [Lawyers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalConsultations_Lookups_ConsultationStatusLookupId] FOREIGN KEY ([ConsultationStatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalConsultations_Lookups_ConsultationTypeLookupId] FOREIGN KEY ([ConsultationTypeLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LegalConsultations_Lookups_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Lookups] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [FeeInstallments] (
        [Id] int NOT NULL IDENTITY,
        [FeeAgreementId] int NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [StatusLookupId] int NOT NULL,
        [PaidDate] datetime2 NULL,
        CONSTRAINT [PK_FeeInstallments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeeInstallments_FeeAgreements_FeeAgreementId] FOREIGN KEY ([FeeAgreementId]) REFERENCES [FeeAgreements] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FeeInstallments_Lookups_StatusLookupId] FOREIGN KEY ([StatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [ExecutionCases] (
        [Id] int NOT NULL IDENTITY,
        [JudgmentId] int NOT NULL,
        [ExecutionStatusLookupId] int NOT NULL,
        [ExecutionOfficer] nvarchar(200) NULL,
        [ExecutionNotes] nvarchar(2000) NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_ExecutionCases] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExecutionCases_Judgments_JudgmentId] FOREIGN KEY ([JudgmentId]) REFERENCES [Judgments] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ExecutionCases_Lookups_ExecutionStatusLookupId] FOREIGN KEY ([ExecutionStatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [MeetingTasks] (
        [Id] int NOT NULL IDENTITY,
        [MeetingId] int NOT NULL,
        [TaskId] int NOT NULL,
        CONSTRAINT [PK_MeetingTasks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MeetingTasks_LegalTasks_TaskId] FOREIGN KEY ([TaskId]) REFERENCES [LegalTasks] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MeetingTasks_Meetings_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [Meetings] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [TreasuryTransactions] (
        [Id] int NOT NULL IDENTITY,
        [TreasuryId] int NOT NULL,
        [TransactionTypeLookupId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Notes] nvarchar(2000) NULL,
        [TransactionDate] datetime2 NOT NULL,
        [CaseId] int NULL,
        CONSTRAINT [PK_TreasuryTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreasuryTransactions_Cases_CaseId] FOREIGN KEY ([CaseId]) REFERENCES [Cases] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_TreasuryTransactions_Lookups_TransactionTypeLookupId] FOREIGN KEY ([TransactionTypeLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TreasuryTransactions_OfficeTreasuries_TreasuryId] FOREIGN KEY ([TreasuryId]) REFERENCES [OfficeTreasuries] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE TABLE [ContractVersions] (
        [Id] int NOT NULL IDENTITY,
        [ContractId] int NOT NULL,
        [VersionNumber] int NOT NULL,
        [FilePath] nvarchar(500) NOT NULL,
        [Notes] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_ContractVersions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ContractVersions_Contracts_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Contracts] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Lawyers_BranchId] ON [Lawyers] ([BranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Lawyers_DepartmentId] ON [Lawyers] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Expenses_ApprovedByUserId] ON [Expenses] ([ApprovedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Expenses_StatusLookupId] ON [Expenses] ([StatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Expenses_SubmittedByUserId] ON [Expenses] ([SubmittedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Clients_BranchId] ON [Clients] ([BranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Cases_BranchId] ON [Cases] ([BranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Cases_DepartmentId] ON [Cases] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Cases_WorkflowStageLookupId] ON [Cases] ([WorkflowStageLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_BranchId] ON [AspNetUsers] ([BranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Branches_ManagerUserId] ON [Branches] ([ManagerUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseAssignmentHistories_CaseId] ON [CaseAssignmentHistories] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseAssignmentHistories_ChangedByUserId] ON [CaseAssignmentHistories] ([ChangedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseAssignmentHistories_LawyerId] ON [CaseAssignmentHistories] ([LawyerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseInternalNotes_CaseId] ON [CaseInternalNotes] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseInternalNotes_CreatedByUserId] ON [CaseInternalNotes] ([CreatedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseStageHistories_CaseId] ON [CaseStageHistories] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseStageHistories_ChangedByUserId] ON [CaseStageHistories] ([ChangedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseStageHistories_FromStageLookupId] ON [CaseStageHistories] ([FromStageLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_CaseStageHistories_ToStageLookupId] ON [CaseStageHistories] ([ToStageLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_ConflictChecks_CaseId] ON [ConflictChecks] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_ConflictChecks_CheckedByUserId] ON [ConflictChecks] ([CheckedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_ConflictChecks_ResultStatusLookupId] ON [ConflictChecks] ([ResultStatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Contracts_AssignedLawyerId] ON [Contracts] ([AssignedLawyerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Contracts_BranchId] ON [Contracts] ([BranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Contracts_ClientId] ON [Contracts] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Contracts_ContractTypeLookupId] ON [Contracts] ([ContractTypeLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Contracts_DepartmentId] ON [Contracts] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Contracts_StatusLookupId] ON [Contracts] ([StatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_ContractVersions_ContractId] ON [ContractVersions] ([ContractId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_ExecutionCases_ExecutionStatusLookupId] ON [ExecutionCases] ([ExecutionStatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_ExecutionCases_JudgmentId] ON [ExecutionCases] ([JudgmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_FeeAgreements_CaseId] ON [FeeAgreements] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_FeeAgreements_ClientId] ON [FeeAgreements] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_FeeAgreements_FeeTypeLookupId] ON [FeeAgreements] ([FeeTypeLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_FeeInstallments_FeeAgreementId] ON [FeeInstallments] ([FeeAgreementId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_FeeInstallments_StatusLookupId] ON [FeeInstallments] ([StatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Judgments_CaseId] ON [Judgments] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Judgments_CourtLevelLookupId] ON [Judgments] ([CourtLevelLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalConsultations_AssignedLawyerId] ON [LegalConsultations] ([AssignedLawyerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalConsultations_BranchId] ON [LegalConsultations] ([BranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalConsultations_ClientId] ON [LegalConsultations] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalConsultations_ConsultationStatusLookupId] ON [LegalConsultations] ([ConsultationStatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalConsultations_ConsultationTypeLookupId] ON [LegalConsultations] ([ConsultationTypeLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalConsultations_DepartmentId] ON [LegalConsultations] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalTasks_AssignedToUserId] ON [LegalTasks] ([AssignedToUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalTasks_CreatedByUserId] ON [LegalTasks] ([CreatedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalTasks_PriorityLookupId] ON [LegalTasks] ([PriorityLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalTasks_RelatedCaseId] ON [LegalTasks] ([RelatedCaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalTasks_StatusLookupId] ON [LegalTasks] ([StatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_LegalTasks_TaskTypeLookupId] ON [LegalTasks] ([TaskTypeLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Meetings_AssignedUserId] ON [Meetings] ([AssignedUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Meetings_CaseId] ON [Meetings] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Meetings_ClientId] ON [Meetings] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_Meetings_StatusLookupId] ON [Meetings] ([StatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_MeetingTasks_MeetingId] ON [MeetingTasks] ([MeetingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_MeetingTasks_TaskId] ON [MeetingTasks] ([TaskId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_PowerOfAttorneys_CaseId] ON [PowerOfAttorneys] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_PowerOfAttorneys_ClientId] ON [PowerOfAttorneys] ([ClientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_PowerOfAttorneys_StatusLookupId] ON [PowerOfAttorneys] ([StatusLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_PowerOfAttorneys_TypeLookupId] ON [PowerOfAttorneys] ([TypeLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_TreasuryTransactions_CaseId] ON [TreasuryTransactions] ([CaseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_TreasuryTransactions_TransactionTypeLookupId] ON [TreasuryTransactions] ([TransactionTypeLookupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    CREATE INDEX [IX_TreasuryTransactions_TreasuryId] ON [TreasuryTransactions] ([TreasuryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD CONSTRAINT [FK_Cases_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD CONSTRAINT [FK_Cases_Lookups_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Lookups] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Cases] ADD CONSTRAINT [FK_Cases_Lookups_WorkflowStageLookupId] FOREIGN KEY ([WorkflowStageLookupId]) REFERENCES [Lookups] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Clients] ADD CONSTRAINT [FK_Clients_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD CONSTRAINT [FK_Expenses_AspNetUsers_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD CONSTRAINT [FK_Expenses_AspNetUsers_SubmittedByUserId] FOREIGN KEY ([SubmittedByUserId]) REFERENCES [AspNetUsers] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Expenses] ADD CONSTRAINT [FK_Expenses_Lookups_StatusLookupId] FOREIGN KEY ([StatusLookupId]) REFERENCES [Lookups] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Lawyers] ADD CONSTRAINT [FK_Lawyers_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    ALTER TABLE [Lawyers] ADD CONSTRAINT [FK_Lawyers_Lookups_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Lookups] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624081411_EnterpriseExpansion_20260624'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624081411_EnterpriseExpansion_20260624', N'8.0.6');
END;
GO

COMMIT;
GO

