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

CREATE TABLE [Clientes] (
    [ClienteId] uniqueidentifier NOT NULL,
    [Contrasena] nvarchar(256) NOT NULL,
    [Estado] bit NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Genero] nvarchar(20) NULL,
    [Edad] int NOT NULL,
    [Identificacion] nvarchar(20) NOT NULL,
    [Direccion] nvarchar(200) NULL,
    [Telefono] nvarchar(20) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([ClienteId])
);
GO

CREATE UNIQUE INDEX [IX_Clientes_Identificacion] ON [Clientes] ([Identificacion]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260919142014_InitialRefactorUsuarios', N'8.0.11');
GO

COMMIT;
GO

