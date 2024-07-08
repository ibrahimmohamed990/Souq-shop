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
    WHERE [MigrationId] = N'20240606024242_Init'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CreatedTime] datetime2 NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240606024242_Init'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240606024242_Init', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240611161142_AddProductTable'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [ImageUrl] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240611161142_AddProductTable'
)
BEGIN
    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240611161142_AddProductTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240611161142_AddProductTable', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624011358_AddShoppingCartTable'
)
BEGIN
    CREATE TABLE [ShoppingCarts] (
        [Id] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [Count] int NOT NULL,
        [UserId] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ShoppingCarts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShoppingCarts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624011358_AddShoppingCartTable'
)
BEGIN
    CREATE INDEX [IX_ShoppingCarts_ProductId] ON [ShoppingCarts] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624011358_AddShoppingCartTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240624011358_AddShoppingCartTable', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624030807_RenameIdColumnInShoppingCartTable'
)
BEGIN
    EXEC sp_rename N'[ShoppingCarts].[Id]', N'ShoppingCartId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624030807_RenameIdColumnInShoppingCartTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240624030807_RenameIdColumnInShoppingCartTable', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626040241_AddOrdersTables'
)
BEGIN
    CREATE TABLE [OrderHeaders] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [ShippingDate] datetime2 NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [OrderStatus] nvarchar(max) NULL,
        [PaymentStatus] nvarchar(max) NULL,
        [TrackingNumber] nvarchar(max) NULL,
        [Carrier] nvarchar(max) NULL,
        [PaymentDate] datetime2 NOT NULL,
        [SessionId] nvarchar(max) NULL,
        [PaymentIntentId] nvarchar(max) NULL,
        [UserName] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_OrderHeaders] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626040241_AddOrdersTables'
)
BEGIN
    CREATE TABLE [OrderDetails] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [Count] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderDetails_OrderHeaders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [OrderHeaders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626040241_AddOrdersTables'
)
BEGIN
    CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626040241_AddOrdersTables'
)
BEGIN
    CREATE INDEX [IX_OrderDetails_ProductId] ON [OrderDetails] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626040241_AddOrdersTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240626040241_AddOrdersTables', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240627054833_UpdateNullableValueInOrderTables'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderHeaders]') AND [c].[name] = N'ShippingDate');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [OrderHeaders] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [OrderHeaders] ALTER COLUMN [ShippingDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240627054833_UpdateNullableValueInOrderTables'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderHeaders]') AND [c].[name] = N'PaymentDate');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [OrderHeaders] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [OrderHeaders] ALTER COLUMN [PaymentDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240627054833_UpdateNullableValueInOrderTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240627054833_UpdateNullableValueInOrderTables', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240702213245_ClearOrdersTable'
)
BEGIN
    DELETE FROM OrderHeaders
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240702213245_ClearOrdersTable'
)
BEGIN
    DELETE FROM OrderDetails
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240702213245_ClearOrdersTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240702213245_ClearOrdersTable', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240703040102_ReAddShoppingCartTable'
)
BEGIN
    CREATE TABLE [ShoppingCarts] (
        [ShoppingCartId] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [Count] int NOT NULL,
        [UserId] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ShoppingCarts] PRIMARY KEY ([ShoppingCartId]),
        CONSTRAINT [FK_ShoppingCarts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240703040102_ReAddShoppingCartTable'
)
BEGIN
    CREATE INDEX [IX_ShoppingCarts_ProductId] ON [ShoppingCarts] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240703040102_ReAddShoppingCartTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240703040102_ReAddShoppingCartTable', N'8.0.6');
END;
GO

COMMIT;
GO

