using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class SgmMesaiService : ISgmMesaiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SgmMesaiService> _logger;

        public SgmMesaiService(
            IUnitOfWork unitOfWork,
            ILogger<SgmMesaiService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<SgmMesaiReportDto>> GetSgmMesaiReportAsync(SgmMesaiFilterRequestDto request)
        {
            try
            {
                // 1. SGM bilgisini al
                var sgmRepo = _unitOfWork.Repository<Sgm>();
                var sgm = await sgmRepo.GetByIdAsync(request.SgmId);

                if (sgm == null)
                {
                    return ApiResponseDto<SgmMesaiReportDto>.ErrorResult($"SGM bulunamadı: {request.SgmId}");
                }

                // 2. SGM'ye bağlı personelleri al
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var tumPersoneller = await personelRepo.GetAllWithDetailsAsync();

                var personeller = tumPersoneller
                    .Where(p => p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif &&
                               p.Departman != null &&
                               p.Departman.SgmId == request.SgmId &&
                               (!request.ServisId.HasValue || p.ServisId == request.ServisId.Value))
                    .ToList();

                if (!personeller.Any())
                {
                    return ApiResponseDto<SgmMesaiReportDto>.ErrorResult("Belirtilen kriterlerde personel bulunamadı");
                }

                // 3. Tüm personeller için CekilenData kayıtlarını çek
                var cekilenDataRepo = _unitOfWork.GetRepository<ICekilenDataRepository>();
                var tumCekilenData = await cekilenDataRepo.GetByDateRangeAsync(request.BaslangicTarihi, request.BitisTarihi);

                // 4. İzin/Mazeret kayıtlarını çek
                var izinMazeretRepo = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var tumIzinMazeretler = await izinMazeretRepo.GetByDateRangeAsync(
                    request.BaslangicTarihi,
                    request.BitisTarihi);

                // 5. Her personel için özet oluştur
                var personelOzetleri = new List<PersonelMesaiOzetDto>();

                foreach (var personel in personeller)
                {
                    var kayitNo = personel.PersonelKayitNo.ToString();
                    var personelCekilenData = tumCekilenData
                        .Where(x => x.KayitNo == kayitNo && x.Tarih.HasValue)
                        .OrderBy(x => x.Tarih)
                        .ToList();

                    var personelIzinMazeretler = tumIzinMazeretler
                        .Where(im => im.TcKimlikNo == personel.TcKimlikNo)
                        .ToList();

                    var ozet = ProcessPersonelMesai(
                        personel,
                        personelCekilenData,
                        personelIzinMazeretler,
                        request.BaslangicTarihi,
                        request.BitisTarihi);

                    personelOzetleri.Add(ozet);
                }

                // 6. Raporu oluştur
                var servisAdi = request.ServisId.HasValue
                    ? personeller.FirstOrDefault()?.Servis?.ServisAdi
                    : null;

                var report = new SgmMesaiReportDto
                {
                    SgmAdi = sgm.SgmAdi,
                    ServisAdi = servisAdi,
                    BaslangicTarihi = request.BaslangicTarihi,
                    BitisTarihi = request.BitisTarihi,
                    Personeller = personelOzetleri.OrderBy(p => p.AdSoyad).ToList()
                };

                _logger.LogInformation(
                    "SGM mesai raporu oluşturuldu: {SgmAdi}, {PersonelSayisi} personel",
                    sgm.SgmAdi,
                    personelOzetleri.Count);

                return ApiResponseDto<SgmMesaiReportDto>.SuccessResult(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SGM mesai raporu oluşturulurken hata oluştu: SgmId={SgmId}", request.SgmId);
                return ApiResponseDto<SgmMesaiReportDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        private PersonelMesaiOzetDto ProcessPersonelMesai(
            Personel personel,
            List<CekilenData> cekilenDataList,
            List<IzinMazeretTalep> izinMazeretList,
            DateTime baslangicTarihi,
            DateTime bitisTarihi)
        {
            var gunlukDetaylar = new List<PersonelMesaiGunlukDto>();
            var toplamGun = 0;
            var calistigiGun = 0;
            var izinliGun = 0;
            var mazeretliGun = 0;
            var devamsizGun = 0;
            var haftaSonuCalisma = 0;
            var gecKalma = 0;
            var toplamMesaiDakika = 0;

            for (var date = baslangicTarihi.Date; date <= bitisTarihi.Date; date = date.AddDays(1))
            {
                toplamGun++;
                var gunlukKayitlar = cekilenDataList.Where(x => x.Tarih.Value.Date == date).ToList();
                var izinMazeret = izinMazeretList.FirstOrDefault(im =>
                    (im.BaslangicTarihi.HasValue && im.BitisTarihi.HasValue &&
                     date >= im.BaslangicTarihi.Value.Date && date <= im.BitisTarihi.Value.Date) ||
                    (im.MazeretTarihi.HasValue && date == im.MazeretTarihi.Value.Date));

                var gunlukDto = new PersonelMesaiGunlukDto
                {
                    Tarih = date,
                    HaftaSonu = IsWeekend(date)
                };

                // İzin/Mazeret kontrolü
                if (izinMazeret != null)
                {
                    gunlukDto.Durum = izinMazeret.Turu.ToString();
                    gunlukDto.MesaiSuresi = "-";

                    if (izinMazeret.Turu.ToString().Contains("İzin"))
                        izinliGun++;
                    else
                        mazeretliGun++;
                }
                // Kayıt yoksa
                else if (!gunlukKayitlar.Any())
                {
                    gunlukDto.Durum = gunlukDto.HaftaSonu ? "Hafta Sonu" : "Devamsız";
                    gunlukDto.MesaiSuresi = "-";

                    if (!gunlukDto.HaftaSonu)
                        devamsizGun++;
                }
                // Giriş/Çıkış var
                else
                {
                    var girisKayit = gunlukKayitlar
                        .Where(x => x.GirisCikisModu == "0")
                        .OrderBy(x => x.Tarih)
                        .FirstOrDefault();

                    var cikisKayit = gunlukKayitlar
                        .Where(x => x.GirisCikisModu == "1")
                        .OrderByDescending(x => x.Tarih)
                        .FirstOrDefault();

                    if (girisKayit?.Tarih != null)
                    {
                        gunlukDto.GirisSaati = girisKayit.Tarih.Value.TimeOfDay;

                        // Geç kalma kontrolü
                        var normalGirisSaati = new TimeSpan(8, 15, 0);
                        if (gunlukDto.GirisSaati > normalGirisSaati)
                        {
                            gunlukDto.GecKalma = true;
                            gecKalma++;
                        }
                    }

                    if (cikisKayit?.Tarih != null)
                    {
                        gunlukDto.CikisSaati = cikisKayit.Tarih.Value.TimeOfDay;
                    }

                    // Mesai süresi hesaplama
                    if (gunlukDto.GirisSaati.HasValue && gunlukDto.CikisSaati.HasValue)
                    {
                        var sure = gunlukDto.CikisSaati.Value - gunlukDto.GirisSaati.Value;
                        if (sure.TotalMinutes > 0)
                        {
                            toplamMesaiDakika += (int)sure.TotalMinutes;
                            gunlukDto.MesaiSuresi = $"{(int)sure.TotalHours:D2}:{sure.Minutes:D2}";
                            calistigiGun++;
                        }
                        else
                        {
                            gunlukDto.MesaiSuresi = "00:00";
                        }
                    }
                    else
                    {
                        gunlukDto.MesaiSuresi = "-";
                    }

                    gunlukDto.Durum = gunlukDto.HaftaSonu ? "Hafta Sonu Çalışma" : "Çalıştı";

                    if (gunlukDto.HaftaSonu && gunlukKayitlar.Any())
                    {
                        haftaSonuCalisma++;
                    }
                }

                gunlukDetaylar.Add(gunlukDto);
            }

            // Toplam mesai süresini formatla
            var toplamSaat = toplamMesaiDakika / 60;
            var toplamDakika = toplamMesaiDakika % 60;
            var toplamMesaiSuresi = $"{toplamSaat:D2}:{toplamDakika:D2}";

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
                GunlukDetay = gunlukDetaylar
            };
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
