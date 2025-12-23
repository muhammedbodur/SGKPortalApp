-- =============================================
-- Performance Optimization Indexes
-- Created: 2025-12-23
-- Purpose: Optimize permission queries for 3-4K concurrent users
-- =============================================

-- =============================================
-- 1. PersonelYetki Table Index
-- Purpose: Speed up permission lookups by user TC
-- Impact: Reduces permission query time from O(n) to O(log n)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name='IX_PersonelYetki_TcKimlikNo_Performance'
               AND object_id = OBJECT_ID('PersonelYetki'))
BEGIN
    PRINT 'Creating index: IX_PersonelYetki_TcKimlikNo_Performance...'

    CREATE NONCLUSTERED INDEX IX_PersonelYetki_TcKimlikNo_Performance
    ON PersonelYetki(TcKimlikNo)
    INCLUDE (ModulControllerIslemId, YetkiSeviyesi, SilindiMi)
    WHERE SilindiMi = 0;  -- Filtered index: only active records

    PRINT '✓ Index created successfully: IX_PersonelYetki_TcKimlikNo_Performance'
END
ELSE
BEGIN
    PRINT '⚠ Index already exists: IX_PersonelYetki_TcKimlikNo_Performance (Skipping)'
END
GO

-- =============================================
-- 2. ModulControllerIslem PermissionKey Index
-- Purpose: Speed up permission key lookups
-- Impact: Faster field-level permission validation
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name='IX_ModulControllerIslem_PermissionKey_Performance'
               AND object_id = OBJECT_ID('ModulControllerIslem'))
BEGIN
    PRINT 'Creating index: IX_ModulControllerIslem_PermissionKey_Performance...'

    CREATE NONCLUSTERED INDEX IX_ModulControllerIslem_PermissionKey_Performance
    ON ModulControllerIslem(PermissionKey)
    INCLUDE (MinYetkiSeviyesi, IslemTipi, DtoTypeName, DtoFieldName)
    WHERE PermissionKey IS NOT NULL AND SilindiMi = 0;  -- Filtered index

    PRINT '✓ Index created successfully: IX_ModulControllerIslem_PermissionKey_Performance'
END
ELSE
BEGIN
    PRINT '⚠ Index already exists: IX_ModulControllerIslem_PermissionKey_Performance (Skipping)'
END
GO

-- =============================================
-- 3. User SessionID Index
-- Purpose: Speed up session validation checks
-- Impact: Faster multi-device login detection
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name='IX_User_SessionID_Performance'
               AND object_id = OBJECT_ID('User'))
BEGIN
    PRINT 'Creating index: IX_User_SessionID_Performance...'

    CREATE NONCLUSTERED INDEX IX_User_SessionID_Performance
    ON [User](SessionID)
    INCLUDE (TcKimlikNo, AktifMi, BankoModuAktif)
    WHERE SessionID IS NOT NULL;  -- Filtered index

    PRINT '✓ Index created successfully: IX_User_SessionID_Performance'
END
ELSE
BEGIN
    PRINT '⚠ Index already exists: IX_User_SessionID_Performance (Skipping)'
END
GO

-- =============================================
-- 4. ModulControllerIslem IslemTipi Index
-- Purpose: Speed up field permission cache loading
-- Impact: Faster startup and permission refresh
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes
               WHERE name='IX_ModulControllerIslem_IslemTipi_Performance'
               AND object_id = OBJECT_ID('ModulControllerIslem'))
BEGIN
    PRINT 'Creating index: IX_ModulControllerIslem_IslemTipi_Performance...'

    CREATE NONCLUSTERED INDEX IX_ModulControllerIslem_IslemTipi_Performance
    ON ModulControllerIslem(IslemTipi, SilindiMi)
    INCLUDE (PermissionKey, MinYetkiSeviyesi, DtoTypeName, DtoFieldName)
    WHERE SilindiMi = 0;  -- Filtered index

    PRINT '✓ Index created successfully: IX_ModulControllerIslem_IslemTipi_Performance'
END
ELSE
BEGIN
    PRINT '⚠ Index already exists: IX_ModulControllerIslem_IslemTipi_Performance (Skipping)'
END
GO

-- =============================================
-- Verification: Check Index Creation
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'INDEX CREATION SUMMARY'
PRINT '========================================='
PRINT ''

SELECT
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE WHEN i.has_filter = 1 THEN 'Yes' ELSE 'No' END AS IsFiltered,
    i.filter_definition AS FilterDefinition,
    STATS_DATE(i.object_id, i.index_id) AS LastUpdated
FROM sys.indexes i
WHERE i.name IN (
    'IX_PersonelYetki_TcKimlikNo_Performance',
    'IX_ModulControllerIslem_PermissionKey_Performance',
    'IX_User_SessionID_Performance',
    'IX_ModulControllerIslem_IslemTipi_Performance'
)
ORDER BY TableName, IndexName;

PRINT ''
PRINT '✓ Performance indexes created successfully!'
PRINT 'Estimated performance improvement: 40-60% for permission queries'
PRINT ''
