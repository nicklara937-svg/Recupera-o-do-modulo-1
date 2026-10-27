/* =====================================================================
   TaskFlow API - Script de criação do banco de dados (SQL Server)
   Equivalente à migration InitialCreate do Entity Framework Core.

   Como executar (qualquer uma das opções):
     - SSMS / Azure Data Studio: abrir o arquivo e executar (F5)
     - sqlcmd: sqlcmd -S localhost,1433 -U sa -P "TaskFlow@123" -C -i Database/script.sql
   ===================================================================== */

IF DB_ID(N'TaskFlowDb') IS NULL
BEGIN
    CREATE DATABASE [TaskFlowDb];
END;
GO

USE [TaskFlowDb];
GO

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

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261026120000_InitialCreate')
BEGIN
    CREATE TABLE [Categorias] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
    );

    CREATE TABLE [Tarefas] (
        [Id] int NOT NULL IDENTITY,
        [Titulo] nvarchar(200) NOT NULL,
        [Descricao] nvarchar(1000) NULL,
        [Status] nvarchar(20) NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataConclusao] datetime2 NULL,
        CONSTRAINT [PK_Tarefas] PRIMARY KEY ([Id])
    );

    CREATE TABLE [Chamados] (
        [Id] int NOT NULL IDENTITY,
        [Titulo] nvarchar(200) NOT NULL,
        [Descricao] nvarchar(1000) NULL,
        [DataCriacao] datetime2 NOT NULL,
        [TarefaId] int NOT NULL,
        [CategoriaId] int NULL,
        CONSTRAINT [PK_Chamados] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Chamados_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Chamados_Tarefas_TarefaId] FOREIGN KEY ([TarefaId]) REFERENCES [Tarefas] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [Interacoes] (
        [Id] int NOT NULL IDENTITY,
        [Mensagem] nvarchar(1000) NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [ChamadoId] int NOT NULL,
        CONSTRAINT [PK_Interacoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Interacoes_Chamados_ChamadoId] FOREIGN KEY ([ChamadoId]) REFERENCES [Chamados] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Chamados_CategoriaId] ON [Chamados] ([CategoriaId]);
    CREATE INDEX [IX_Chamados_TarefaId] ON [Chamados] ([TarefaId]);
    CREATE INDEX [IX_Interacoes_ChamadoId] ON [Interacoes] ([ChamadoId]);
    CREATE INDEX [IX_Tarefas_Status] ON [Tarefas] ([Status]);
END;
GO

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261026120000_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261026120000_InitialCreate', N'10.0.0');
END;
GO

COMMIT;
GO
