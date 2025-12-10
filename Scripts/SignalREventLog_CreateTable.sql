-- ═══════════════════════════════════════════════════════
-- SignalR Event Log Tablosu
-- Gönderilen tüm SignalR mesajlarının kaydını tutar
-- ═══════════════════════════════════════════════════════

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SIG_EventLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SIG_EventLogs] (
        [EventLogId] BIGINT IDENTITY(1,1) NOT NULL,
        [EventId] UNIQUEIDENTIFIER NOT NULL,
        [EventType] INT NOT NULL,
        [EventName] NVARCHAR(100) NOT NULL,
        [TargetType] INT NOT NULL,
        [TargetId] NVARCHAR(500) NULL,
        [TargetCount] INT NOT NULL DEFAULT 0,
        [SiraId] INT NULL,
        [SiraNo] INT NULL,
        [BankoId] INT NULL,
        [TvId] INT NULL,
        [PersonelTc] NVARCHAR(11) NULL,
        [PayloadSummary] NVARCHAR(2000) NULL,
        [DeliveryStatus] INT NOT NULL DEFAULT 0,
        [ErrorMessage] NVARCHAR(500) NULL,
        [SentAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [AcknowledgedAt] DATETIME2 NULL,
        [DurationMs] INT NULL,
        [HizmetBinasiId] INT NULL,
        [CorrelationId] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SIG_EventLogs] PRIMARY KEY CLUSTERED ([EventLogId] ASC)
    );

    PRINT '✅ SIG_EventLogs tablosu oluşturuldu';
END
ELSE
BEGIN
    PRINT 'ℹ️ SIG_EventLogs tablosu zaten mevcut';
END
GO

-- ═══════════════════════════════════════════════════════
-- INDEXES
-- ═══════════════════════════════════════════════════════

-- EventId (Unique)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_EventId')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_SIG_EventLogs_EventId] 
    ON [dbo].[SIG_EventLogs] ([EventId] ASC);
    PRINT '✅ IX_SIG_EventLogs_EventId index oluşturuldu';
END
GO

-- SentAt (Tarih bazlı sorgular için)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_SentAt')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_SentAt] 
    ON [dbo].[SIG_EventLogs] ([SentAt] DESC);
    PRINT '✅ IX_SIG_EventLogs_SentAt index oluşturuldu';
END
GO

-- EventType
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_EventType')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_EventType] 
    ON [dbo].[SIG_EventLogs] ([EventType] ASC);
    PRINT '✅ IX_SIG_EventLogs_EventType index oluşturuldu';
END
GO

-- DeliveryStatus
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_DeliveryStatus')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_DeliveryStatus] 
    ON [dbo].[SIG_EventLogs] ([DeliveryStatus] ASC);
    PRINT '✅ IX_SIG_EventLogs_DeliveryStatus index oluşturuldu';
END
GO

-- SiraId (Filtered)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_SiraId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_SiraId] 
    ON [dbo].[SIG_EventLogs] ([SiraId] ASC)
    WHERE [SiraId] IS NOT NULL;
    PRINT '✅ IX_SIG_EventLogs_SiraId index oluşturuldu';
END
GO

-- BankoId (Filtered)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_BankoId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_BankoId] 
    ON [dbo].[SIG_EventLogs] ([BankoId] ASC)
    WHERE [BankoId] IS NOT NULL;
    PRINT '✅ IX_SIG_EventLogs_BankoId index oluşturuldu';
END
GO

-- TvId (Filtered)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_TvId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_TvId] 
    ON [dbo].[SIG_EventLogs] ([TvId] ASC)
    WHERE [TvId] IS NOT NULL;
    PRINT '✅ IX_SIG_EventLogs_TvId index oluşturuldu';
END
GO

-- PersonelTc (Filtered)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_PersonelTc')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_PersonelTc] 
    ON [dbo].[SIG_EventLogs] ([PersonelTc] ASC)
    WHERE [PersonelTc] IS NOT NULL;
    PRINT '✅ IX_SIG_EventLogs_PersonelTc index oluşturuldu';
END
GO

-- CorrelationId (Filtered)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_CorrelationId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_CorrelationId] 
    ON [dbo].[SIG_EventLogs] ([CorrelationId] ASC)
    WHERE [CorrelationId] IS NOT NULL;
    PRINT '✅ IX_SIG_EventLogs_CorrelationId index oluşturuldu';
END
GO

-- Composite Index (Ortak filtreleme için)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIG_EventLogs_SentAt_EventType_Status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_SIG_EventLogs_SentAt_EventType_Status] 
    ON [dbo].[SIG_EventLogs] ([SentAt] DESC, [EventType] ASC, [DeliveryStatus] ASC);
    PRINT '✅ IX_SIG_EventLogs_SentAt_EventType_Status index oluşturuldu';
END
GO

PRINT '';
PRINT '═══════════════════════════════════════════════════════';
PRINT '✅ SignalR Event Log tablosu ve indexleri hazır!';
PRINT '═══════════════════════════════════════════════════════';
