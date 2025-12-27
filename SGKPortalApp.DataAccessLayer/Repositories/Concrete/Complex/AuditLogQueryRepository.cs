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
                query = query.Where(l => l.TabloAdi == filter.TableName);

            // İşlem türü filtresi
            if (filter.Action.HasValue)
                query = query.Where(l => l.IslemTuru == filter.Action.Value);

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
    }
}
