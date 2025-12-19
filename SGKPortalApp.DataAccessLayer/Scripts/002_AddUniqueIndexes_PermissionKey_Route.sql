-- =============================================
-- Script: Add Unique Indexes for PermissionKey and Route
-- Date: 2025-12-19
-- Description: Adds filtered unique indexes for PermissionKey and Route columns
--              with SilindiMi = 0 filter to support soft delete
-- =============================================

USE [SGKPortalDB]
GO

-- =============================================
-- 1. Create Unique Index on PermissionKey
-- =============================================
IF NOT EXISTS (
    SELECT * FROM sys.indexes
    WHERE name = 'IX_PER_ModulControllerIslemleri_PermissionKey'
    AND object_id = OBJECT_ID(N'[dbo].[PER_ModulControllerIslemleri]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_PER_ModulControllerIslemleri_PermissionKey]
    ON [dbo].[PER_ModulControllerIslemleri] ([PermissionKey])
    WHERE ([SilindiMi] = 0)

    PRINT '✅ Unique index IX_PER_ModulControllerIslemleri_PermissionKey created successfully'
END
ELSE
BEGIN
    PRINT '⚠️  Index IX_PER_ModulControllerIslemleri_PermissionKey already exists'
END
GO

-- =============================================
-- 2. Create Unique Index on Route
-- =============================================
IF NOT EXISTS (
    SELECT * FROM sys.indexes
    WHERE name = 'IX_PER_ModulControllerIslemleri_Route'
    AND object_id = OBJECT_ID(N'[dbo].[PER_ModulControllerIslemleri]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_PER_ModulControllerIslemleri_Route]
    ON [dbo].[PER_ModulControllerIslemleri] ([Route])
    WHERE ([SilindiMi] = 0 AND [Route] IS NOT NULL)

    PRINT '✅ Unique index IX_PER_ModulControllerIslemleri_Route created successfully'
END
ELSE
BEGIN
    PRINT '⚠️  Index IX_PER_ModulControllerIslemleri_Route already exists'
END
GO

PRINT '🎉 Migration completed successfully!'
PRINT ''
PRINT '📋 Summary:'
PRINT '  - PermissionKey: Unique among non-deleted records'
PRINT '  - Route: Unique among non-deleted records (NULL values allowed)'
PRINT '  - A deleted record can be recreated with the same PermissionKey or Route'
GO
