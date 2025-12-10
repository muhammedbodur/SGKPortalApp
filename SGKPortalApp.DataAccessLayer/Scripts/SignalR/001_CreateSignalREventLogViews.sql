-- ═══════════════════════════════════════════════════════
-- SignalR Event Log View'ları - VARSA ELLEME
-- Migration: AddSignalREventLogTableAndViews
-- Tarih: 2025-12-10
-- ═══════════════════════════════════════════════════════

-- View'lar zaten varsa hiçbir şey yapma
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_SignalREventLogs')
BEGIN
    PRINT N'✅ SignalR Event Log View''ları zaten mevcut - atlanıyor';
END
ELSE
BEGIN
    PRINT N'📝 SignalR Event Log View''ları oluşturuluyor...';
END
