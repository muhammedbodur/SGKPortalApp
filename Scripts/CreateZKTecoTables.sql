-- ================================================
-- ZKTeco Tabloları Oluşturma Script'i
-- SGKPortalApp - PDKS Realtime Entegrasyon
-- ================================================

USE [SGKDB] -- Veritabanı adınızı buraya yazın
GO

-- ================================================
-- 1. ZKTeco_Device Tablosu
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZKTeco_Device]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZKTeco_Device](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [DeviceName] [nvarchar](250) NULL,
        [IpAddress] [nvarchar](50) NOT NULL,
        [Port] [nvarchar](50) NULL DEFAULT ('4370'),
        [DeviceCode] [nvarchar](50) NULL,
        [DeviceInfo] [nvarchar](255) NULL,
        [IsActive] [bit] NOT NULL DEFAULT (1),

        -- Sync İşlemleri
        [LastSyncTime] [datetime2](7) NULL,
        [SyncCount] [int] NOT NULL DEFAULT (0),
        [LastSyncSuccess] [bit] NULL,
        [LastSyncStatus] [nvarchar](max) NULL,

        -- Health Check
        [LastHealthCheckTime] [datetime2](7) NULL,
        [HealthCheckCount] [int] NOT NULL DEFAULT (0),
        [LastHealthCheckSuccess] [bit] NULL,
        [LastHealthCheckStatus] [nvarchar](max) NULL,

        -- Audit
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT (GETDATE()),

        CONSTRAINT [PK_ZKTeco_Device] PRIMARY KEY CLUSTERED ([Id] ASC)
    )

    PRINT '✓ ZKTeco_Device tablosu oluşturuldu'
END
ELSE
BEGIN
    PRINT '○ ZKTeco_Device tablosu zaten mevcut'
END
GO

-- ================================================
-- 2. ZKTeco_User Tablosu
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZKTeco_User]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZKTeco_User](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [EnrollNumber] [nvarchar](50) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Password] [nvarchar](50) NULL,
        [CardNumber] [bigint] NULL,
        [Privilege] [int] NOT NULL DEFAULT (0),
        [Enabled] [bit] NOT NULL DEFAULT (1),
        [DeviceId] [int] NULL,
        [DeviceIp] [nvarchar](50) NULL,
        [LastSyncTime] [datetime2](7) NULL,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT (GETDATE()),

        CONSTRAINT [PK_ZKTeco_User] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ZKTeco_User_Device] FOREIGN KEY([DeviceId])
            REFERENCES [dbo].[ZKTeco_Device] ([Id]) ON DELETE SET NULL
    )

    -- Unique constraint: EnrollNumber + DeviceId kombinasyonu unique olmalı
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ZKTeco_User_EnrollNumber_DeviceId]
    ON [dbo].[ZKTeco_User] ([EnrollNumber], [DeviceId])
    WHERE [DeviceId] IS NOT NULL

    PRINT '✓ ZKTeco_User tablosu oluşturuldu'
END
ELSE
BEGIN
    PRINT '○ ZKTeco_User tablosu zaten mevcut'
END
GO

-- ================================================
-- 3. ZKTeco_CekilenData Tablosu
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZKTeco_CekilenData]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZKTeco_CekilenData](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SiraNo] [int] NULL,
        [KayitNo] [nvarchar](50) NULL,
        [Dogrulama] [nvarchar](50) NULL,
        [GirisCikisModu] [nvarchar](50) NULL,
        [Tarih] [datetime2](7) NULL,
        [Saat] [time](7) NULL,
        [WorkCode] [nvarchar](50) NULL,
        [Reserved] [nvarchar](50) NULL,
        [BosBaslik] [nvarchar](max) NULL,
        [CihazAdi] [nvarchar](50) NULL,
        [DeviceId] [int] NULL,

        -- Modern Field'lar
        [CihazIp] [nvarchar](50) NULL,
        [CekilmeTarihi] [datetime2](7) NOT NULL DEFAULT (GETDATE()),
        [IsProcessed] [bit] NOT NULL DEFAULT (0),
        [ProcessedAt] [datetime2](7) NULL,
        [RawData] [nvarchar](max) NULL,

        CONSTRAINT [PK_ZKTeco_CekilenData] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ZKTeco_CekilenData_Device] FOREIGN KEY([DeviceId])
            REFERENCES [dbo].[ZKTeco_Device] ([Id]) ON DELETE SET NULL
    )

    PRINT '✓ ZKTeco_CekilenData tablosu oluşturuldu'
END
ELSE
BEGIN
    PRINT '○ ZKTeco_CekilenData tablosu zaten mevcut'
END
GO

-- ================================================
-- 4. Index'ler (Performans için)
-- ================================================

-- User için index'ler
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_User_EnrollNumber' AND object_id = OBJECT_ID('ZKTeco_User'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_User_EnrollNumber]
    ON [dbo].[ZKTeco_User] ([EnrollNumber])
    PRINT '✓ Index IX_ZKTeco_User_EnrollNumber oluşturuldu'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_User_CardNumber' AND object_id = OBJECT_ID('ZKTeco_User'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_User_CardNumber]
    ON [dbo].[ZKTeco_User] ([CardNumber])
    WHERE [CardNumber] IS NOT NULL
    PRINT '✓ Index IX_ZKTeco_User_CardNumber oluşturuldu'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_User_DeviceId' AND object_id = OBJECT_ID('ZKTeco_User'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_User_DeviceId]
    ON [dbo].[ZKTeco_User] ([DeviceId])
    PRINT '✓ Index IX_ZKTeco_User_DeviceId oluşturuldu'
END

-- CekilenData için index'ler
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_CekilenData_KayitNo' AND object_id = OBJECT_ID('ZKTeco_CekilenData'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_CekilenData_KayitNo]
    ON [dbo].[ZKTeco_CekilenData] ([KayitNo])
    PRINT '✓ Index IX_ZKTeco_CekilenData_KayitNo oluşturuldu'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_CekilenData_Tarih' AND object_id = OBJECT_ID('ZKTeco_CekilenData'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_CekilenData_Tarih]
    ON [dbo].[ZKTeco_CekilenData] ([Tarih])
    PRINT '✓ Index IX_ZKTeco_CekilenData_Tarih oluşturuldu'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_CekilenData_IsProcessed' AND object_id = OBJECT_ID('ZKTeco_CekilenData'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_CekilenData_IsProcessed]
    ON [dbo].[ZKTeco_CekilenData] ([IsProcessed])
    PRINT '✓ Index IX_ZKTeco_CekilenData_IsProcessed oluşturuldu'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_CekilenData_DeviceId' AND object_id = OBJECT_ID('ZKTeco_CekilenData'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_CekilenData_DeviceId]
    ON [dbo].[ZKTeco_CekilenData] ([DeviceId])
    PRINT '✓ Index IX_ZKTeco_CekilenData_DeviceId oluşturuldu'
END

-- Device için index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_Device_IpAddress' AND object_id = OBJECT_ID('ZKTeco_Device'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_Device_IpAddress]
    ON [dbo].[ZKTeco_Device] ([IpAddress])
    PRINT '✓ Index IX_ZKTeco_Device_IpAddress oluşturuldu'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZKTeco_Device_IsActive' AND object_id = OBJECT_ID('ZKTeco_Device'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ZKTeco_Device_IsActive]
    ON [dbo].[ZKTeco_Device] ([IsActive])
    PRINT '✓ Index IX_ZKTeco_Device_IsActive oluşturuldu'
END
GO

-- ================================================
-- 5. Örnek Test Verisi (Opsiyonel)
-- ================================================

-- Örnek cihaz ekle (eğer yoksa)
DECLARE @TestDeviceId INT

IF NOT EXISTS (SELECT * FROM [dbo].[ZKTeco_Device] WHERE [IpAddress] = '192.168.1.201')
BEGIN
    INSERT INTO [dbo].[ZKTeco_Device]
        ([DeviceName], [IpAddress], [Port], [DeviceCode], [IsActive], [CreatedAt], [UpdatedAt])
    VALUES
        ('Test ZKTeco Cihazı', '192.168.1.201', '4370', 'DEV001', 1, GETDATE(), GETDATE())

    SET @TestDeviceId = SCOPE_IDENTITY()

    PRINT '✓ Örnek test cihazı eklendi (IP: 192.168.1.201)'
    PRINT '  Not: Bu örnek cihazı kendi cihazınızın bilgileriyle güncelleyin'

    -- Örnek kullanıcı ekle (eğer cihaz yeni eklendiyse)
    IF NOT EXISTS (SELECT * FROM [dbo].[ZKTeco_User] WHERE [EnrollNumber] = 'TEST001' AND [DeviceId] = @TestDeviceId)
    BEGIN
        INSERT INTO [dbo].[ZKTeco_User]
            ([EnrollNumber], [Name], [Password], [CardNumber], [Privilege], [Enabled], [DeviceId], [DeviceIp], [CreatedAt], [UpdatedAt])
        VALUES
            ('TEST001', 'Test Kullanıcı', '12345', 1234567890, 0, 1, @TestDeviceId, '192.168.1.201', GETDATE(), GETDATE())

        PRINT '✓ Örnek test kullanıcısı eklendi (EnrollNumber: TEST001)'
    END
END
GO

PRINT ''
PRINT '================================================'
PRINT '✓ ZKTeco tabloları başarıyla oluşturuldu!'
PRINT '================================================'
PRINT ''
PRINT 'Sonraki Adımlar:'
PRINT '1. ZKTeco_Device tablosuna gerçek cihaz bilgilerinizi ekleyin'
PRINT '2. SGKPortalApp.ApiLayer uygulamasını yeniden başlatın'
PRINT '3. /pdks/giris-cikis/realtime sayfasını açın'
PRINT '4. ZKTeco cihazlarında kart okutarak test edin'
PRINT ''
GO
