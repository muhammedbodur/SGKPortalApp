using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Complex
{
    /// <summary>
    /// Audit log modülü için kompleks sorguları barındıran repository
    /// Yetki bazlı filtreleme ve kullanıcı bilgisi join'leri içerir
    /// </summary>
    public class AuditLogQueryRepository : IAuditLogQueryRepository
    {
        private readonly SGKDbContext _context;

        public AuditLogQueryRepository(SGKDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Yetki bazlı filtreleme ile audit log'ları getirir
        /// </summary>
        public async Task<(List<DatabaseLog> Logs, int TotalCount)> GetLogsWithPermissionFilterAsync(
            AuditLogFilterDto filter,
            int? userDepartmanId,
            int? userServisId)
        {
            var query = _context.DatabaseLogs.AsQueryable();

            query = query.Where(l => l.ChangedFieldCount.HasValue && l.ChangedFieldCount > 0);

            // ═══════════════════════════════════════════════════════════
            // ✅ KATMAN 1: GÜVENLİK - YETKİ BAZLI KISITLAMA (EN ÖNCELİKLİ)
            // Bu filtre MUTLAKA uygulanır, kullanıcı bypass edemez
            // ═══════════════════════════════════════════════════════════

            // Eğer kullanıcının departman kısıtlaması varsa
            if (userDepartmanId.HasValue)
            {
                // Sadece kendi departmanındaki kullanıcıların loglarını görebilir
                query = query.Where(l =>
                    _context.Users
                        .Where(u => u.TcKimlikNo == l.TcKimlikNo)
                        .Where(u => u.Personel != null && u.Personel.DepartmanId == userDepartmanId.Value)
                        .Any()
                );
            }

            // Eğer kullanıcının servis kısıtlaması varsa
            if (userServisId.HasValue)
            {
                query = query.Where(l =>
                    _context.Users
                        .Where(u => u.TcKimlikNo == l.TcKimlikNo)
                        .Where(u => u.Personel != null && u.Personel.ServisId == userServisId.Value)
                        .Any()
                );
            }

            // ═══════════════════════════════════════════════════════════
            // ✅ KATMAN 2: KULLANICI FİLTRELERİ (İsteğe bağlı daralma)
            // Yukarıdaki güvenlik kısıtlaması içinde arama yapar
            // ═══════════════════════════════════════════════════════════

            // Departman filtresi (isteğe bağlı daralma)
            if (filter.DepartmanId.HasValue)
            {
                query = query.Where(l =>
                    _context.Users
                        .Where(u => u.TcKimlikNo == l.TcKimlikNo)
                        .Where(u => u.Personel != null && u.Personel.DepartmanId == filter.DepartmanId.Value)
                        .Any()
                );
            }

            // Servis filtresi (isteğe bağlı daralma)
            if (filter.ServisId.HasValue)
            {
                query = query.Where(l =>
                    _context.Users
                        .Where(u => u.TcKimlikNo == l.TcKimlikNo)
                        .Where(u => u.Personel != null && u.Personel.ServisId == filter.ServisId.Value)
                        .Any()
                );
            }

            // Tarih filtreleri
            if (filter.StartDate.HasValue)
                query = query.Where(l => l.IslemZamani >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.IslemZamani <= filter.EndDate.Value);

            // TC Kimlik No filtresi
            if (!string.IsNullOrWhiteSpace(filter.TcKimlikNo))
                query = query.Where(l => l.TcKimlikNo == filter.TcKimlikNo);

            // Ad Soyad / Sicil No arama (SearchText)
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                query = query.Where(l =>
                    _context.Users
                        .Where(u => u.TcKimlikNo == l.TcKimlikNo)
                        .Where(u => u.Personel != null &&
                                   (u.Personel.AdSoyad.Contains(filter.SearchText) ||
                                    u.Personel.SicilNo.ToString().Contains(filter.SearchText)))
                        .Any()
                );
            }

            // Tablo adı filtresi
            if (!string.IsNullOrWhiteSpace(filter.TableName))
                query = query.Where(l => l.TableName == filter.TableName);

            // İşlem türü filtresi
            if (filter.Action.HasValue)
                query = query.Where(l => l.DatabaseAction == filter.Action.Value);

            // Transaction ID filtresi
            if (filter.TransactionId.HasValue)
                query = query.Where(l => l.TransactionId == filter.TransactionId.Value);

            // Storage type filtreleri
            if (filter.OnlyFileBased == true)
                query = query.Where(l => l.StorageType == LogStorageType.File);

            if (filter.OnlyDatabaseBased == true)
                query = query.Where(l => l.StorageType == LogStorageType.Database);

            // Total count
            var totalCount = await query.CountAsync();

            // Pagination ve sıralama
            var logs = await query
                .OrderByDescending(l => l.IslemZamani)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (logs, totalCount);
        }

        /// <summary>
        /// Log detayını getirir ve yetki kontrolü yapar
        /// </summary>
        public async Task<DatabaseLog?> GetLogWithPermissionCheckAsync(
            int logId,
            int? userDepartmanId,
            int? userServisId)
        {
            var log = await _context.DatabaseLogs
                .Where(l => l.DatabaseLogId == logId)
                .FirstOrDefaultAsync();

            if (log == null)
                return null;

            // ═══════════════════════════════════════════════════════════
            // ✅ YETKİ KONTROLÜ: Kullanıcı bu log'u görüntüleyebilir mi?
            // ═══════════════════════════════════════════════════════════

            // Eğer kullanıcının departman kısıtlaması varsa
            if (userDepartmanId.HasValue)
            {
                // Log'un sahibi kullanıcının departmanını kontrol et
                var hasAccess = await _context.Users
                    .Where(u => u.TcKimlikNo == log.TcKimlikNo)
                    .Where(u => u.Personel != null && u.Personel.DepartmanId == userDepartmanId.Value)
                    .AnyAsync();

                if (!hasAccess)
                    return null; // Yetkisiz erişim
            }

            // Eğer kullanıcının servis kısıtlaması varsa
            if (userServisId.HasValue)
            {
                var hasAccess = await _context.Users
                    .Where(u => u.TcKimlikNo == log.TcKimlikNo)
                    .Where(u => u.Personel != null && u.Personel.ServisId == userServisId.Value)
                    .AnyAsync();

                if (!hasAccess)
                    return null; // Yetkisiz erişim
            }

            return log;
        }

        public async Task<List<DatabaseLog>> GetTransactionLogsAsync(Guid transactionId)
        {
            return await _context.DatabaseLogs
                .Where(l => l.TransactionId == transactionId)
                .OrderBy(l => l.IslemZamani)
                .ToListAsync();
        }

        public async Task<List<DatabaseLog>> GetUserRecentLogsAsync(string tcKimlikNo, int count)
        {
            return await _context.DatabaseLogs
                .Where(l => l.TcKimlikNo == tcKimlikNo)
                .OrderByDescending(l => l.IslemZamani)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<DatabaseLog>> GetEntityHistoryAsync(string tableName, string entityId)
        {
            return await _context.DatabaseLogs
                .Where(l => l.TableName == tableName)
                .Where(l => (l.BeforeData != null && l.BeforeData.Contains(entityId)) ||
                           (l.AfterData != null && l.AfterData.Contains(entityId)))
                .OrderBy(l => l.IslemZamani)
                .ToListAsync();
        }

        public async Task<List<DatabaseLog>> GetRelatedLogsInTransactionAsync(Guid transactionId, int excludeLogId)
        {
            return await _context.DatabaseLogs
                .Where(l => l.TransactionId == transactionId && l.DatabaseLogId != excludeLogId)
                .OrderBy(l => l.IslemZamani)
                .ToListAsync();
        }

        /// <summary>
        /// Field value için FK lookup yaparak kullanıcı dostu gösterim döndürür
        /// </summary>
        public async Task<string?> ResolveFieldValueAsync(string fieldName, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // ID parse et
            if (!int.TryParse(value, out int id))
                return null;

            try
            {
                // FK field'larını tespit et ve lookup yap
                return fieldName switch
                {
                    // Organizasyon
                    "DepartmanId" => await _context.Departmanlar
                        .Where(d => d.DepartmanId == id)
                        .Select(d => d.DepartmanAdi)
                        .FirstOrDefaultAsync(),

                    "ServisId" => await _context.Servisler
                        .Where(s => s.ServisId == id)
                        .Select(s => s.ServisAdi)
                        .FirstOrDefaultAsync(),

                    "UnvanId" => await _context.Unvanlar
                        .Where(u => u.UnvanId == id)
                        .Select(u => u.UnvanAdi)
                        .FirstOrDefaultAsync(),

                    "SendikaId" => await _context.Sendikalar
                        .Where(s => s.SendikaId == id)
                        .Select(s => s.SendikaAdi)
                        .FirstOrDefaultAsync(),

                    "AtanmaNedeniId" => await _context.AtanmaNedenleri
                        .Where(a => a.AtanmaNedeniId == id)
                        .Select(a => a.AtanmaNedeni)
                        .FirstOrDefaultAsync(),

                    // Lokasyon
                    "HizmetBinasiId" => await _context.HizmetBinalari
                        .Where(h => h.HizmetBinasiId == id)
                        .Select(h => h.HizmetBinasiAdi)
                        .FirstOrDefaultAsync(),

                    "IlId" or "EsininIsIlId" => await _context.Iller
                        .Where(i => i.IlId == id)
                        .Select(i => i.IlAdi)
                        .FirstOrDefaultAsync(),

                    "IlceId" or "EsininIsIlceId" => await _context.Ilceler
                        .Where(i => i.IlceId == id)
                        .Select(i => i.IlceAdi)
                        .FirstOrDefaultAsync(),

                    // Yetki/Modül Sistemi
                    "ModulId" => await _context.Moduller
                        .Where(m => m.ModulId == id)
                        .Select(m => m.ModulAdi)
                        .FirstOrDefaultAsync(),

                    "ModulControllerId" or "UstModulControllerId" => await _context.ModulControllers
                        .Where(m => m.ModulControllerId == id)
                        .Select(m => m.ModulControllerAdi)
                        .FirstOrDefaultAsync(),

                    "ModulControllerIslemId" or "UstIslemId" => await _context.ModulControllerIslemleri
                        .Where(m => m.ModulControllerIslemId == id)
                        .Select(m => m.ModulControllerIslemAdi + " (" + m.PermissionKey + ")")
                        .FirstOrDefaultAsync(),

                    // Sıramatik - Banko
                    "BankoId" or "YonlendirenBankoId" or "HedefBankoId" or "AktifBankoId" => await _context.Bankolar
                        .Include(b => b.HizmetBinasi)
                        .Where(b => b.BankoId == id)
                        .Select(b => "Banko #" + b.BankoNo + " (" + b.HizmetBinasi.HizmetBinasiAdi + ")")
                        .FirstOrDefaultAsync(),

                    // Sıramatik - Tv
                    "TvId" => await _context.Tvler
                        .Where(t => t.TvId == id)
                        .Select(t => t.TvAdi)
                        .FirstOrDefaultAsync(),

                    // Sıramatik - Kiosk
                    "KioskId" => await _context.Kiosklar
                        .Where(k => k.KioskId == id)
                        .Select(k => k.KioskAdi)
                        .FirstOrDefaultAsync(),

                    // Sıramatik - Sıra
                    "SiraId" => await _context.Siralar
                        .Where(s => s.SiraId == id)
                        .Select(s => "Sıra #" + s.SiraNo)
                        .FirstOrDefaultAsync(),

                    // Sıramatik - Kanal
                    "KanalId" => await _context.Kanallar
                        .Where(k => k.KanalId == id)
                        .Select(k => k.KanalAdi)
                        .FirstOrDefaultAsync(),

                    // Sıramatik - KanalAlt
                    "KanalAltId" => await _context.KanallarAlt
                        .Where(k => k.KanalAltId == id)
                        .Select(k => k.KanalAltAdi)
                        .FirstOrDefaultAsync(),

                    // Sıramatik - KanalIslem
                    "KanalIslemId" => await _context.KanalIslemleri
                        .Where(k => k.KanalIslemId == id)
                        .Select(k => k.Kanal.KanalAdi)
                        .FirstOrDefaultAsync(),

                    // Sıramatik - KanalAltIslem
                    "KanalAltIslemId" => await _context.KanalAltIslemleri
                        .Where(k => k.KanalAltIslemId == id)
                        .Select(k => k.KanalAlt.KanalAltAdi)
                        .FirstOrDefaultAsync(),

                    //// PDKS
                    //"PdksCihazId" => await _context.PdksCihazlar
                    //    .Include(p => p.Departman)
                    //    .Where(p => p.PdksCihazId == id)
                    //    .Select(p => p.CihazIP + " (" + p.Departman.DepartmanAdi + ")")
                    //    .FirstOrDefaultAsync(),

                    _ => null
                };
            }
            catch
            {
                // Lookup hatası - null dön
                return null;
            }
        }

        /// <summary>
        /// TC Kimlik No'dan Ad Soyad'ı getirir
        /// </summary>
        public async Task<string?> GetAdSoyadByTcKimlikNoAsync(string? tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            try
            {
                return await _context.Personeller
                    .Where(p => p.TcKimlikNo == tcKimlikNo)
                    .Select(p => p.AdSoyad)
                    .FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
