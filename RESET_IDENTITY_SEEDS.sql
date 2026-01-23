-- Database'i temizleyip identity seed'leri resetlemek için SQL script
-- Bu script'i SSMS'de çalıştırın

USE SGKPortalDB;
GO

-- Tüm tabloları temizle
DELETE FROM CMN_HubConnections;
DELETE FROM CMN_Users;
DELETE FROM PER_Personeller;
DELETE FROM CMN_HizmetBinalari;
DELETE FROM PER_AtanmaNedenleri;
DELETE FROM PER_Unvanlar;
DELETE FROM PER_Servisler;
DELETE FROM PER_Departmanlar;
GO

-- Identity seed'leri resetle
DBCC CHECKIDENT ('PER_Departmanlar', RESEED, 0);
DBCC CHECKIDENT ('PER_Servisler', RESEED, 0);
DBCC CHECKIDENT ('PER_Unvanlar', RESEED, 0);
DBCC CHECKIDENT ('CMN_HizmetBinalari', RESEED, 0);
DBCC CHECKIDENT ('PER_AtanmaNedenleri', RESEED, 0);
GO

PRINT '✅ Database temizlendi ve identity seed''ler resetlendi!';
PRINT 'Şimdi API''yi başlatın - seeding otomatik çalışacak.';
