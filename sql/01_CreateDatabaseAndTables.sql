-- ============================================================================
-- SQL Script: 01_CreateDatabaseAndTables.sql
-- Description: Creates Database, Tables, Primary Keys, Foreign Keys, and Indexes
-- Target DB: SQL Server 2019 / 2022 / LocalDB
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StudentManagementDb')
BEGIN
    CREATE DATABASE StudentManagementDb;
END
GO

USE StudentManagementDb;
GO

-- 1. Create Users Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL,
        [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Users_CreatedDate] DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 2. Create Students Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Students]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Students] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [Age] INT NOT NULL,
        [Course] NVARCHAR(100) NOT NULL,
        [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Students_CreatedDate] DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_Students_Age] CHECK ([Age] > 0)
    );
END
GO

-- 3. Create Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Users_Username' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users] ([Username] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Students_Email' AND object_id = OBJECT_ID(N'[dbo].[Students]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Students_Email] ON [dbo].[Students] ([Email] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Students_Name' AND object_id = OBJECT_ID(N'[dbo].[Students]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Students_Name] ON [dbo].[Students] ([Name] ASC);
END
GO
