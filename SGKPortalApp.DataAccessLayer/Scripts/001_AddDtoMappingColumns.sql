-- =============================================
-- Script: Add DTO Mapping Columns to PER_ModulControllerIslemleri
-- Date: 2025-12-16
-- Description: Adds DtoTypeName and DtoFieldName columns for field-level permission validation
-- =============================================

USE [SGKPortal]
GO

-- Check if columns already exist
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[PER_ModulControllerIslemleri]')
    AND name = 'DtoTypeName'
)
BEGIN
    ALTER TABLE [dbo].[PER_ModulControllerIslemleri]
    ADD [DtoTypeName] NVARCHAR(200) NULL

    PRINT '✅ DtoTypeName column added successfully'
END
ELSE
BEGIN
    PRINT '⚠️  DtoTypeName column already exists'
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[PER_ModulControllerIslemleri]')
    AND name = 'DtoFieldName'
)
BEGIN
    ALTER TABLE [dbo].[PER_ModulControllerIslemleri]
    ADD [DtoFieldName] NVARCHAR(100) NULL

    PRINT '✅ DtoFieldName column added successfully'
END
ELSE
BEGIN
    PRINT '⚠️  DtoFieldName column already exists'
END
GO

PRINT '🎉 Migration completed successfully!'
GO
