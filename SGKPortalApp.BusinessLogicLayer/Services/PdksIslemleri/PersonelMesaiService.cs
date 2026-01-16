using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class PersonelMesaiService : IPersonelMesaiService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonelMesaiService> _logger;

        public PersonelMesaiService(
            ApplicationDbContext context,
            ILogger<PersonelMesaiService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelMesaiListResponseDto>>> GetPersonelMesaiListAsync(PersonelMesaiFilterRequestDto request)
        {
            try
            {
                // Personel bilgisini al
                var personel = await _context.Personeller
                    .Include(p => p.Departman)
                    .Include(p => p.Servis)
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == request.TcKimlikNo);

                if (personel == null)
                    return ApiResponseDto<List<PersonelMesaiListResponseDto>>.ErrorResult("Personel bulunamadı");

                // CekilenData'dan mesai kayıtlarını çek
                var mesaiKayitlari = await _context.CekilenDatalar
                    .Where(c => c.KayitNo == personel.PersonelKayitNo.ToString() &&
                               c.Tarih >= request.BaslangicTarihi &&
                               c.Tarih <= request.BitisTarihi)
                    .OrderBy(c => c.Tarih)
                    .ToListAsync();

                // Günlük bazda grupla (giriş/çıkış eşleştir)
                var gunlukKayitlar = mesaiKayitlari
                    .GroupBy(m => m.Tarih!.Value.Date)
                    .Select(g => new
                    {
                        Tarih = g.Key,
                        Girisler = g.Where(x => x.GirisCikisModu == "0" || x.GirisCikisModu == "I/O").OrderBy(x => x.Tarih).ToList(),
                        Cikislar = g.Where(x => x.GirisCikisModu == "1" || x.GirisCikisModu == "I/O").OrderByDescending(x => x.Tarih).ToList()
                    })
                    .ToList();

                // İzin ve mazeret kayıtlarını çek
                var izinKayitlari = await _context.IzinMazeretTalepleri
                    .Where(i => i.TcKimlikNo == request.TcKimlikNo &&
                               i.BaslangicTarihi >= request.BaslangicTarihi &&
                               i.BitisTarihi <= request.BitisTarihi &&
                               i.Turu != BusinessObjectLayer.Enums.PdksIslemleri.IzinMazeretTuru.Mazeret)
                    .ToListAsync();

                var mazeretKayitlari = await _context.IzinMazeretTalepleri
                    .Where(m => m.TcKimlikNo == request.TcKimlikNo &&
                               m.MazeretTarihi >= request.BaslangicTarihi &&
                               m.MazeretTarihi <= request.BitisTarihi &&
                               m.Turu == BusinessObjectLayer.Enums.PdksIslemleri.IzinMazeretTuru.Mazeret)
                    .ToListAsync();

                // Response DTO'ları oluştur
                var result = new List<PersonelMesaiListResponseDto>();

                foreach (var gunluk in gunlukKayitlar)
                {
                    var girisSaati = gunluk.Girisler.FirstOrDefault()?.Tarih;
                    var cikisSaati = gunluk.Cikislar.FirstOrDefault()?.Tarih;

                    // Mesai süresi hesapla
                    string mesaiSuresi = "-";
                    int? mesaiDakika = null;
                    if (girisSaati.HasValue && cikisSaati.HasValue)
                    {
                        var fark = cikisSaati.Value - girisSaati.Value;
                        mesaiDakika = (int)fark.TotalMinutes;
                        int saat = mesaiDakika.Value / 60;
                        int dakika = mesaiDakika.Value % 60;
                        mesaiSuresi = $"{saat:00}:{dakika:00}";
                    }

                    // Hafta sonu kontrolü
                    bool haftaSonu = gunluk.Tarih.DayOfWeek == DayOfWeek.Saturday ||
                                    gunluk.Tarih.DayOfWeek == DayOfWeek.Sunday;

                    // Geç kalma kontrolü (08:30'dan sonra)
                    bool gecKalma = false;
                    if (girisSaati.HasValue && !haftaSonu)
                    {
                        var toleransSaati = new TimeSpan(8, 30, 59);
                        gecKalma = girisSaati.Value.TimeOfDay > toleransSaati;
                    }

                    // İzin/Mazeret kontrolü
                    string? detay = null;
                    var izin = izinKayitlari.FirstOrDefault(i =>
                        i.BaslangicTarihi <= gunluk.Tarih && i.BitisTarihi >= gunluk.Tarih);
                    var mazeret = mazeretKayitlari.FirstOrDefault(m =>
                        m.MazeretTarihi == gunluk.Tarih);

                    if (mazeret != null)
                        detay = $"{mazeret.Turu} ({mazeret.SaatDilimi})";
                    else if (izin != null)
                        detay = izin.Turu.ToString();
                    else if (haftaSonu)
                        detay = "Hafta Sonu";

                    result.Add(new PersonelMesaiListResponseDto
                    {
                        Tarih = gunluk.Tarih,
                        TcKimlikNo = personel.TcKimlikNo,
                        AdSoyad = personel.AdSoyad,
                        SicilNo = personel.SicilNo,
                        DepartmanAdi = personel.Departman?.DepartmanAdi ?? "",
                        ServisAdi = personel.Servis?.ServisAdi,
                        GirisSaati = girisSaati?.TimeOfDay,
                        CikisSaati = cikisSaati?.TimeOfDay,
                        MesaiSuresi = mesaiSuresi,
                        MesaiSuresiDakika = mesaiDakika,
                        Detay = detay,
                        HaftaSonu = haftaSonu,
                        GecKalma = gecKalma,
                        IzinTipi = izin?.Turu.ToString(),
                        MazeretTipi = mazeret?.Turu.ToString()
                    });
                }

                return ApiResponseDto<List<PersonelMesaiListResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel mesai listesi alınırken hata: {TcKimlikNo}", request.TcKimlikNo);
                return ApiResponseDto<List<PersonelMesaiListResponseDto>>.ErrorResult($"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<PersonelMesaiBaslikDto>> GetPersonelBaslikBilgiAsync(string tcKimlikNo)
        {
            try
            {
                var personel = await _context.Personeller
                    .Include(p => p.Departman)
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == tcKimlikNo);

                if (personel == null)
                    return ApiResponseDto<PersonelMesaiBaslikDto>.ErrorResult("Personel bulunamadı");

                var dto = new PersonelMesaiBaslikDto
                {
                    AdSoyad = personel.AdSoyad,
                    DepartmanAdi = personel.Departman?.DepartmanAdi ?? "",
                    BirimAdi = personel.Departman?.DepartmanAdi,
                    SicilNo = personel.SicilNo
                };

                return ApiResponseDto<PersonelMesaiBaslikDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel başlık bilgisi alınırken hata: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelMesaiBaslikDto>.ErrorResult($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
