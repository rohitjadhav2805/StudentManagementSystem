-- ============================================================================
-- SQL Script: 02_StoredProcedures.sql
-- Description: Stored procedures for Student CRUD operations
-- Target DB: StudentManagementDb
-- ============================================================================

USE StudentManagementDb;
GO

-- 1. GetAllStudents
CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllStudents]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Name], [Email], [Age], [Course], [CreatedDate]
    FROM [dbo].[Students]
    ORDER BY [CreatedDate] DESC;
END
GO

-- 2. GetStudentById
CREATE OR ALTER PROCEDURE [dbo].[sp_GetStudentById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [Name], [Email], [Age], [Course], [CreatedDate]
    FROM [dbo].[Students]
    WHERE [Id] = @Id;
END
GO

-- 3. CreateStudent
CREATE OR ALTER PROCEDURE [dbo].[sp_CreateStudent]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @Age INT,
    @Course NVARCHAR(100),
    @CreatedStudentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Email] = @Email)
    BEGIN
        RAISERROR('Email address already exists.', 16, 1);
        RETURN;
    END

    INSERT INTO [dbo].[Students] ([Name], [Email], [Age], [Course], [CreatedDate])
    VALUES (@Name, @Email, @Age, @Course, GETUTCDATE());

    SET @CreatedStudentId = SCOPE_IDENTITY();
END
GO

-- 4. UpdateStudent
CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateStudent]
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @Age INT,
    @Course NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Id] = @Id)
    BEGIN
        RAISERROR('Student not found.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Email] = @Email AND [Id] <> @Id)
    BEGIN
        RAISERROR('Email address already exists for another student.', 16, 1);
        RETURN;
    END

    UPDATE [dbo].[Students]
    SET [Name] = @Name,
        [Email] = @Email,
        [Age] = @Age,
        [Course] = @Course
    WHERE [Id] = @Id;
END
GO

-- 5. DeleteStudent
CREATE OR ALTER PROCEDURE [dbo].[sp_DeleteStudent]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Id] = @Id)
    BEGIN
        RAISERROR('Student not found.', 16, 1);
        RETURN;
    END

    DELETE FROM [dbo].[Students]
    WHERE [Id] = @Id;
END
GO
