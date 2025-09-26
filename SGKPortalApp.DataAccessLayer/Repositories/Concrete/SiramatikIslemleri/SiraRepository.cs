﻿using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class SiraRepository : GenericRepository<Sira>, ISiraRepository
    {
        public SiraRepository(SGKDbContext context) : base(context) { }

        // Sıra numarası ile sırayı getirir
        public async Task<Sira?> GetBySiraNoAsync(int siraNo, int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .FirstOrDefaultAsync(s => s.SiraNo == siraNo && s.HizmetBinasiId == hizmetBinasiId);
        }

        // Personel bazında sıraları listeler
        public async Task<IEnumerable<Sira>> GetByPersonelAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.TcKimlikNo == tcKimlikNo)
                .ToListAsync();
        }

        // Personelin aktif sırasını getirir
        public async Task<Sira?> GetActiveByPersonelAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .FirstOrDefaultAsync(s => s.TcKimlikNo == tcKimlikNo && s.BeklemeDurum == BeklemeDurum.Beklemede);
        }

        // Hizmet binası bazında sıraları listeler
        public async Task<IEnumerable<Sira>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.HizmetBinasiId == hizmetBinasiId)
                .ToListAsync();
        }

        // Kanal alt işlem bazında sıraları listeler
        public async Task<IEnumerable<Sira>> GetByKanalAltIslemAsync(int kanalAltIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.KanalAltIslemId == kanalAltIslemId)
                .ToListAsync();
        }

        // Tarih aralığına göre sıraları listeler
        public async Task<IEnumerable<Sira>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.SiraAlisZamani >= startDate && s.SiraAlisZamani <= endDate)
                .ToListAsync();
        }

        // Bugünkü sıraları listeler
        public async Task<IEnumerable<Sira>> GetTodayAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.SiraAlisZamani.Date == today)
                .ToListAsync();
        }

        // Aktif sıraları listeler
        public async Task<IEnumerable<Sira>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.BeklemeDurum == BeklemeDurum.Beklemede || s.BeklemeDurum == BeklemeDurum.Cagrildi)
                .ToListAsync();
        }

        // Beklemedeki sıraları listeler
        public async Task<IEnumerable<Sira>> GetWaitingAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.BeklemeDurum == BeklemeDurum.Beklemede)
                .ToListAsync();
        }

        // Tamamlanmış sıraları listeler
        public async Task<IEnumerable<Sira>> GetCompletedAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .Where(s => s.BeklemeDurum == BeklemeDurum.Bitti)
                .ToListAsync();
        }

        // Sırayı detaylı getirir
        public async Task<Sira?> GetWithDetailsAsync(int siraId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .FirstOrDefaultAsync(s => s.SiraId == siraId);
        }

        // Tüm sıraları detaylı listeler
        public async Task<IEnumerable<Sira>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.KanalAltIslem)
                .Include(s => s.HizmetBinasi)
                .Include(s => s.Personel)
                .ToListAsync();
        }

        // Günlük sıra sayısını getirir
        public async Task<int> GetDailySiraCountAsync(DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(s => s.SiraAlisZamani.Date == date.Date);
        }

        // Beklemedeki sıra sayısını getirir
        public async Task<int> GetWaitingSiraCountAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(s => s.BeklemeDurum == BeklemeDurum.Beklemede);
        }

        // Tamamlanmış sıra sayısını getirir
        public async Task<int> GetCompletedSiraCountAsync(DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(s => s.BeklemeDurum == BeklemeDurum.Bitti && s.SiraAlisZamani.Date == date.Date);
        }
    }
}