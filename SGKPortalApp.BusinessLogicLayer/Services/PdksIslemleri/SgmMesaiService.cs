using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Services.Base;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.Common.Results;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class SgmMesaiService : BaseService, ISgmMesaiService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SgmMesaiService> _logger;

        public SgmMesaiService(
            ApplicationDbContext context,
            ILogger<SgmMesaiService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<SgmMesaiReportDto>> GetSgmMesaiReportAsync(SgmMesaiFilterRequestDto request)
        {
            try
            {
                // Get SGM info
                var sgm = await _context.Sgm
                    .FirstOrDefaultAsync(s => s.SgmId == request.SgmId);

                if (sgm == null)
                    return Result<SgmMesaiReportDto>.Failure("SGM bulunamadı");

                // Get Servis info if provided
                string? servisAdi = null;
                if (request.ServisId.HasValue)
                {
                    var servis = await _context.Servis
                        .FirstOrDefaultAsync(s => s.ServisId == request.ServisId.Value);
                    servisAdi = servis?.ServisAdi;
                }

                // Get personnel list based on SGM and optional Servis filter
                var personnelQuery = _context.Personeller
                    .Include(p => p.Departman)
                        .ThenInclude(d => d!.Sgm)
                    .Include(p => p.Servis)
                        .ThenInclude(s => s!.Sgm)
                    .Where(p => p.Departman!.SgmId == request.SgmId ||
                               p.Servis!.SgmId == request.SgmId);

                if (request.ServisId.HasValue)
                {
                    personnelQuery = personnelQuery.Where(p => p.ServisId == request.ServisId.Value);
                }

                var personnelList = await personnelQuery
                    .OrderBy(p => p.AdSoyad)
                    .ToListAsync();

                if (!personnelList.Any())
                    return Result<SgmMesaiReportDto>.Failure("Bu SGM/Servis için personel bulunamadı");

                // Calculate total days in date range
                var totalDays = (request.BitisTarihi.Date - request.BaslangicTarihi.Date).Days + 1;

                // Process each personnel's attendance
                var personelOzetleri = new List<PersonelMesaiOzetDto>();

                foreach (var personel in personnelList)
                {
                    var ozet = await CalculatePersonelMesaiOzetAsync(
                        personel,
                        request.BaslangicTarihi,
                        request.BitisTarihi,
                        totalDays);

                    personelOzetleri.Add(ozet);
                }

                var report = new SgmMesaiReportDto
                {
                    SgmAdi = sgm.SgmAdi,
                    ServisAdi = servisAdi,
                    BaslangicTarihi = request.BaslangicTarihi,
                    BitisTarihi = request.BitisTarihi,
                    Personeller = personelOzetleri
                };

                return Result<SgmMesaiReportDto>.Success(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SGM mesai raporu alınırken hata: {SgmId}", request.SgmId);
                return Result<SgmMesaiReportDto>.Failure($"Bir hata oluştu: {ex.Message}");
            }
        }

        private async Task<PersonelMesaiOzetDto> CalculatePersonelMesaiOzetAsync(
            SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri.Personel personel,
            DateTime baslangicTarihi,
            DateTime bitisTarihi,
            int toplamGun)
        {
            // Get attendance records
            var mesaiKayitlari = await _context.CekilenDatalar
                .Where(c => c.KayitNo == personel.PersonelKayitNo.ToString() &&
                           c.Tarih >= baslangicTarihi &&
                           c.Tarih <= bitisTarihi)
                .OrderBy(c => c.Tarih)
                .ToListAsync();

            // Group by day and match entry/exit
            var gunlukKayitlar = mesaiKayitlari
                .GroupBy(m => m.Tarih!.Value.Date)
                .Select(g => new
                {
                    Tarih = g.Key,
                    Girisler = g.Where(x => x.GirisCikisModu == "0" || x.GirisCikisModu == "I/O").OrderBy(x => x.Tarih).ToList(),
                    Cikislar = g.Where(x => x.GirisCikisModu == "1" || x.GirisCikisModu == "I/O").OrderByDescending(x => x.Tarih).ToList()
                })
                .ToList();

            // Get leave/excuse records
            var izinKayitlari = await _context.IzinMazeretTalepleri
                .Where(i => i.TcKimlikNo == personel.TcKimlikNo &&
                           i.BaslangicTarihi >= baslangicTarihi &&
                           i.BitisTarihi <= bitisTarihi &&
                           i.Turu != BusinessObjectLayer.Enums.PdksIslemleri.IzinMazeretTuru.Mazeret)
                .ToListAsync();

            var mazeretKayitlari = await _context.IzinMazeretTalepleri
                .Where(m => m.TcKimlikNo == personel.TcKimlikNo &&
                           m.MazeretTarihi >= baslangicTarihi &&
                           m.MazeretTarihi <= bitisTarihi &&
                           m.Turu == BusinessObjectLayer.Enums.PdksIslemleri.IzinMazeretTuru.Mazeret)
                .ToListAsync();

            // Calculate statistics
            int calistigiGun = 0;
            int izinliGun = 0;
            int mazeretliGun = 0;
            int haftaSonuCalisma = 0;
            int gecKalma = 0;
            int toplamMesaiDakika = 0;

            var gunlukDetay = new List<PersonelMesaiGunlukDto>();

            // Create a set of all dates in range for checking attendance
            var tumGunler = new HashSet<DateTime>();
            for (var date = baslangicTarihi.Date; date <= bitisTarihi.Date; date = date.AddDays(1))
            {
                tumGunler.Add(date);
            }

            foreach (var gunluk in gunlukKayitlar)
            {
                var girisSaati = gunluk.Girisler.FirstOrDefault()?.Tarih;
                var cikisSaati = gunluk.Cikislar.FirstOrDefault()?.Tarih;

                // Calculate duration
                string mesaiSuresi = "-";
                int? mesaiDakika = null;
                if (girisSaati.HasValue && cikisSaati.HasValue)
                {
                    var fark = cikisSaati.Value - girisSaati.Value;
                    mesaiDakika = (int)fark.TotalMinutes;
                    int saat = mesaiDakika.Value / 60;
                    int dakika = mesaiDakika.Value % 60;
                    mesaiSuresi = $"{saat:00}:{dakika:00}";
                    toplamMesaiDakika += mesaiDakika.Value;
                }

                // Weekend check
                bool haftaSonu = gunluk.Tarih.DayOfWeek == DayOfWeek.Saturday ||
                                gunluk.Tarih.DayOfWeek == DayOfWeek.Sunday;

                // Late arrival check (08:30)
                bool gecKalmaDurum = false;
                if (girisSaati.HasValue && !haftaSonu)
                {
                    var toleransSaati = new TimeSpan(8, 30, 59);
                    gecKalmaDurum = girisSaati.Value.TimeOfDay > toleransSaati;
                    if (gecKalmaDurum) gecKalma++;
                }

                // Leave/Excuse check
                string? durum = null;
                var izin = izinKayitlari.FirstOrDefault(i =>
                    i.BaslangicTarihi <= gunluk.Tarih && i.BitisTarihi >= gunluk.Tarih);
                var mazeret = mazeretKayitlari.FirstOrDefault(m =>
                    m.MazeretTarihi == gunluk.Tarih);

                if (mazeret != null)
                {
                    durum = $"{mazeret.Turu} ({mazeret.SaatDilimi})";
                    mazeretliGun++;
                }
                else if (izin != null)
                {
                    durum = izin.Turu.ToString();
                    izinliGun++;
                }
                else if (haftaSonu)
                {
                    durum = "Hafta Sonu";
                    haftaSonuCalisma++;
                }
                else
                {
                    calistigiGun++;
                }

                gunlukDetay.Add(new PersonelMesaiGunlukDto
                {
                    Tarih = gunluk.Tarih,
                    GirisSaati = girisSaati?.TimeOfDay,
                    CikisSaati = cikisSaati?.TimeOfDay,
                    MesaiSuresi = mesaiSuresi,
                    Durum = durum,
                    HaftaSonu = haftaSonu,
                    GecKalma = gecKalmaDurum
                });

                tumGunler.Remove(gunluk.Tarih);
            }

            // Calculate devamsizlik (absence) - days without any record
            int devamsizGun = tumGunler.Count(d =>
                d.DayOfWeek != DayOfWeek.Saturday &&
                d.DayOfWeek != DayOfWeek.Sunday &&
                !izinKayitlari.Any(i => i.BaslangicTarihi <= d && i.BitisTarihi >= d));

            // Format total mesai duration
            int toplamSaat = toplamMesaiDakika / 60;
            int toplamDakika = toplamMesaiDakika % 60;
            string toplamMesaiSuresi = $"{toplamSaat:00}:{toplamDakika:00}";

            return new PersonelMesaiOzetDto
            {
                TcKimlikNo = personel.TcKimlikNo,
                AdSoyad = personel.AdSoyad,
                SicilNo = personel.SicilNo,
                DepartmanAdi = personel.Departman?.DepartmanAdi ?? "",
                ServisAdi = personel.Servis?.ServisAdi,
                ToplamGun = toplamGun,
                CalistigiGun = calistigiGun,
                IzinliGun = izinliGun,
                MazeretliGun = mazeretliGun,
                DevamsizGun = devamsizGun,
                HaftaSonuCalisma = haftaSonuCalisma,
                GecKalma = gecKalma,
                ToplamMesaiSuresi = toplamMesaiSuresi,
                ToplamMesaiDakika = toplamMesaiDakika,
                GunlukDetay = gunlukDetay.OrderBy(g => g.Tarih).ToList()
            };
        }
    }
}
