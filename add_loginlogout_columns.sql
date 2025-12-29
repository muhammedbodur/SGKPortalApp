-- LoginLogoutLog tablosuna yeni kolonlar ekleniyor
USE [SGKPortalDB]
GO

-- AdSoyad kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'AdSoyad')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [AdSoyad] NVARCHAR(200) NULL
END
GO

-- IpAddress kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'IpAddress')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [IpAddress] NVARCHAR(50) NULL
END
GO

-- UserAgent kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'UserAgent')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [UserAgent] NVARCHAR(500) NULL
END
GO

-- Browser kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'Browser')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [Browser] NVARCHAR(100) NULL
END
GO

-- OperatingSystem kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'OperatingSystem')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [OperatingSystem] NVARCHAR(100) NULL
END
GO

-- DeviceType kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'DeviceType')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [DeviceType] NVARCHAR(50) NULL
END
GO

-- LoginSuccessful kolonu ekle (default true)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'LoginSuccessful')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [LoginSuccessful] BIT NOT NULL DEFAULT 1
END
GO

-- FailureReason kolonu ekle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CMN_LoginLogoutLogs]') AND name = 'FailureReason')
BEGIN
    ALTER TABLE [dbo].[CMN_LoginLogoutLogs]
    ADD [FailureReason] NVARCHAR(500) NULL
END
GO

PRINT 'LoginLogoutLogs tablosu başarıyla güncellendi!'
GO
