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

CREATE TABLE [Titulares] (
    [Id] uniqueidentifier NOT NULL,
    [ClienteId] uniqueidentifier NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Identificacion] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Titulares] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Cuentas] (
    [Id] uniqueidentifier NOT NULL,
    [NumeroCuenta] nvarchar(50) NOT NULL,
    [TipoCuenta] nvarchar(20) NOT NULL,
    [SaldoInicial] decimal(18,4) NOT NULL,
    [Estado] bit NOT NULL,
    [TitularId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Cuentas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cuentas_Titulares_TitularId] FOREIGN KEY ([TitularId]) REFERENCES [Titulares] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Movimientos] (
    [Id] uniqueidentifier NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [TipoMovimiento] nvarchar(20) NOT NULL,
    [Valor] decimal(18,4) NOT NULL,
    [Saldo] decimal(18,4) NOT NULL,
    [CuentaId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Movimientos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Movimientos_Cuentas_CuentaId] FOREIGN KEY ([CuentaId]) REFERENCES [Cuentas] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Cuentas_NumeroCuenta] ON [Cuentas] ([NumeroCuenta]);
GO

CREATE INDEX [IX_Cuentas_TitularId] ON [Cuentas] ([TitularId]);
GO

CREATE INDEX [IX_Movimientos_CuentaId] ON [Movimientos] ([CuentaId]);
GO

CREATE UNIQUE INDEX [IX_Titulares_ClienteId] ON [Titulares] ([ClienteId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260919143054_InitialRefactorFinanciero', N'8.0.11');
GO

COMMIT;
GO

