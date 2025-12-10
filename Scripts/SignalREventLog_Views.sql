-- ═══════════════════════════════════════════════════════
-- SignalR Event Log Görüntüleme View'ları
-- Logları okunabilir formatta gösterir
-- ═══════════════════════════════════════════════════════

-- Event Type Enum değerleri
-- 1=NewSira, 2=SiraCalled, 3=SiraCompleted, 4=SiraCancelled, 5=SiraRedirected
-- 10=PanelUpdate, 20=TvUpdate, 30=BankoModeActivated, 31=BankoModeDeactivated
-- 40=ForceLogout, 50=Announcement, 99=Other

-- Target Type Enum değerleri
-- 1=Connection, 2=Connections, 3=Group, 4=All, 5=Personel, 6=Tv

-- Delivery Status Enum değerleri
-- 0=Pending, 1=Sent, 2=Failed, 3=NoTarget, 10=Acknowledged

-- ═══════════════════════════════════════════════════════
-- Ana Görüntüleme View'ı
-- ═══════════════════════════════════════════════════════
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SignalREventLogs')
    DROP VIEW vw_SignalREventLogs;
GO

CREATE VIEW vw_SignalREventLogs AS
SELECT 
    e.EventLogId,
    e.EventId,
    -- Event Type (Okunabilir)
    CASE e.EventType
        WHEN 1 THEN 'Yeni Sıra'
        WHEN 2 THEN 'Sıra Çağrıldı'
        WHEN 3 THEN 'Sıra Tamamlandı'
        WHEN 4 THEN 'Sıra İptal'
        WHEN 5 THEN 'Sıra Yönlendirildi'
        WHEN 10 THEN 'Panel Güncelleme'
        WHEN 20 THEN 'TV Güncelleme'
        WHEN 30 THEN 'Banko Modu Aktif'
        WHEN 31 THEN 'Banko Modu Pasif'
        WHEN 40 THEN 'Zorla Çıkış'
        WHEN 50 THEN 'Duyuru'
        ELSE 'Diğer'
    END AS EventTypeName,
    e.EventName,
    -- Target Type (Okunabilir)
    CASE e.TargetType
        WHEN 1 THEN 'Connection'
        WHEN 2 THEN 'Connections'
        WHEN 3 THEN 'Group'
        WHEN 4 THEN 'All'
        WHEN 5 THEN 'Personel'
        WHEN 6 THEN 'TV'
        ELSE 'Bilinmiyor'
    END AS TargetTypeName,
    e.TargetId,
    e.TargetCount,
    e.SiraId,
    e.SiraNo,
    e.BankoId,
    CONCAT('Banko ', b.BankoNo) AS BankoAdi,
    e.TvId,
    t.TvAdi,
    e.PersonelTc,
    p.AdSoyad AS PersonelAdSoyad,
    -- Delivery Status (Okunabilir)
    CASE e.DeliveryStatus
        WHEN 0 THEN 'Beklemede'
        WHEN 1 THEN 'Gönderildi'
        WHEN 2 THEN 'Başarısız'
        WHEN 3 THEN 'Hedef Yok'
        WHEN 10 THEN 'Alındı (ACK)'
        ELSE 'Bilinmiyor'
    END AS DeliveryStatusName,
    e.ErrorMessage,
    e.SentAt,
    e.AcknowledgedAt,
    e.DurationMs,
    e.PayloadSummary,
    e.HizmetBinasiId,
    hb.HizmetBinasiAdi,
    e.CorrelationId
FROM [dbo].[SIG_EventLogs] e
LEFT JOIN [dbo].[SIR_Bankolar] b ON e.BankoId = b.BankoId
LEFT JOIN [dbo].[SIR_Tvler] t ON e.TvId = t.TvId
LEFT JOIN [dbo].[PER_Personeller] p ON e.PersonelTc = p.TcKimlikNo
LEFT JOIN [dbo].[CMN_HizmetBinalari] hb ON e.HizmetBinasiId = hb.HizmetBinasiId;
GO

PRINT '✅ vw_SignalREventLogs view oluşturuldu';

-- ═══════════════════════════════════════════════════════
-- Son Eventler View'ı (Son 1 saat)
-- ═══════════════════════════════════════════════════════
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SignalREventLogs_Recent')
    DROP VIEW vw_SignalREventLogs_Recent;
GO

CREATE VIEW vw_SignalREventLogs_Recent AS
SELECT TOP 1000 *
FROM vw_SignalREventLogs
WHERE SentAt >= DATEADD(HOUR, -1, GETDATE())
ORDER BY SentAt DESC;
GO

PRINT '✅ vw_SignalREventLogs_Recent view oluşturuldu';

-- ═══════════════════════════════════════════════════════
-- Başarısız Eventler View'ı
-- ═══════════════════════════════════════════════════════
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SignalREventLogs_Failed')
    DROP VIEW vw_SignalREventLogs_Failed;
GO

CREATE VIEW vw_SignalREventLogs_Failed AS
SELECT *
FROM vw_SignalREventLogs
WHERE DeliveryStatusName IN ('Başarısız', 'Hedef Yok');
GO

PRINT '✅ vw_SignalREventLogs_Failed view oluşturuldu';

-- ═══════════════════════════════════════════════════════
-- Özet İstatistikler View'ı
-- ═══════════════════════════════════════════════════════
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SignalREventLogs_Stats')
    DROP VIEW vw_SignalREventLogs_Stats;
GO

CREATE VIEW vw_SignalREventLogs_Stats AS
SELECT 
    CAST(SentAt AS DATE) AS Tarih,
    EventTypeName,
    DeliveryStatusName,
    COUNT(*) AS EventSayisi,
    AVG(DurationMs) AS OrtSureMs,
    MAX(DurationMs) AS MaxSureMs,
    SUM(TargetCount) AS ToplamHedef
FROM vw_SignalREventLogs
GROUP BY CAST(SentAt AS DATE), EventTypeName, DeliveryStatusName;
GO

PRINT '✅ vw_SignalREventLogs_Stats view oluşturuldu';

-- ═══════════════════════════════════════════════════════
-- Sıra Bazlı Event Takibi View'ı
-- ═══════════════════════════════════════════════════════
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SignalREventLogs_BySira')
    DROP VIEW vw_SignalREventLogs_BySira;
GO

CREATE VIEW vw_SignalREventLogs_BySira AS
SELECT 
    SiraId,
    SiraNo,
    STRING_AGG(EventTypeName, ' → ') WITHIN GROUP (ORDER BY SentAt) AS EventAkisi,
    COUNT(*) AS ToplamEvent,
    MIN(SentAt) AS IlkEvent,
    MAX(SentAt) AS SonEvent,
    DATEDIFF(SECOND, MIN(SentAt), MAX(SentAt)) AS ToplamSureSn
FROM vw_SignalREventLogs
WHERE SiraId IS NOT NULL
GROUP BY SiraId, SiraNo;
GO

PRINT '✅ vw_SignalREventLogs_BySira view oluşturuldu';

PRINT '';
PRINT '═══════════════════════════════════════════════════════';
PRINT '✅ Tüm SignalR Event Log view''ları hazır!';
PRINT '';
PRINT 'Kullanım:';
PRINT '  SELECT * FROM vw_SignalREventLogs WHERE SiraNo = 1003';
PRINT '  SELECT * FROM vw_SignalREventLogs_Recent';
PRINT '  SELECT * FROM vw_SignalREventLogs_Failed';
PRINT '  SELECT * FROM vw_SignalREventLogs_Stats';
PRINT '  SELECT * FROM vw_SignalREventLogs_BySira WHERE SiraId = 171';
PRINT '═══════════════════════════════════════════════════════';
