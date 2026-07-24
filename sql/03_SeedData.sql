-- ============================================================================
-- SQL Script: 03_SeedData.sql
-- Description: Inserts initial seed data for Users and Students
-- Target DB: StudentManagementDb
-- Passwords:
--   admin@zestindia.com -> Admin@123
--   user@zestindia.com  -> User@123
-- ============================================================================

USE StudentManagementDb;
GO

-- Seed Users
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@zestindia.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [CreatedDate])
    VALUES ('admin', 'admin@zestindia.com', 'lF3bV19J3t/uN3xK3v8Q1A==:k7lM8p9Q0r1S2t3U4v5W6x7Y8z9A0b1C2d3E4f5G6h7=', 'Admin', GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'user@zestindia.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [Role], [CreatedDate])
    VALUES ('user', 'user@zestindia.com', 'mG4cW20K4u/vO4yL4w9R2B==:l8mN9q0R1s2T3u4V5w6X7y8Z9a0B1c2D3e4F5g6H7i8=', 'User', GETUTCDATE());
END
GO

-- Seed Students
IF NOT EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Email] = 'rahul.sharma@example.com')
BEGIN
    INSERT INTO [dbo].[Students] ([Name], [Email], [Age], [Course], [CreatedDate])
    VALUES ('Rahul Sharma', 'rahul.sharma@example.com', 22, 'Computer Science', DATEADD(day, -30, GETUTCDATE()));
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Email] = 'priya.patel@example.com')
BEGIN
    INSERT INTO [dbo].[Students] ([Name], [Email], [Age], [Course], [CreatedDate])
    VALUES ('Priya Patel', 'priya.patel@example.com', 21, 'Information Technology', DATEADD(day, -25, GETUTCDATE()));
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Students] WHERE [Email] = 'amit.kumar@example.com')
BEGIN
    INSERT INTO [dbo].[Students] ([Name], [Email], [Age], [Course], [CreatedDate])
    VALUES ('Amit Kumar', 'amit.kumar@example.com', 23, 'Software Engineering', DATEADD(day, -20, GETUTCDATE()));
END
GO
