using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class PersonelMesaiService : IPersonelMesaiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PersonelMesaiService> _logger;

        public PersonelMesaiService(
            IUnitOfWork unitOfWork,
            ILogger<PersonelMesaiService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<PersonelMesaiListResponseDto>>> GetPersonelMesaiListAsync(PersonelMesaiFilterRequestDto request)
        {
            try
            {
                // 1. Personel bilgisini al (KayitNo için)
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(request.TcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<List<PersonelMesaiListResponseDto>>.ErrorResult($"Personel bulunamadı: {request.TcKimlikNo}");
                }

                var kayitNo = personel.PersonelKayitNo.ToString();

                // 2. CekilenData kayıtlarını çek
                var cekilenDataRepo = _unitOfWork.GetRepository<ICekilenDataRepository>();
                var cekilenData = await cekilenDataRepo.GetByDateRangeAsync(request.BaslangicTarihi, request.BitisTarihi);

                // KayitNo'ya göre filtrele (string karşılaştırma)
                var personelData = cekilenData
                    .Where(x => !string.IsNullOrEmpty(x.KayitNo) && 
                               x.KayitNo == kayitNo && 
                               x.Tarih.HasValue)
                    .OrderBy(x => x.Tarih)
                    .ToList();

                _logger.LogInformation(
                    "Personel mesai verileri: TcKimlikNo={TcKimlikNo}, KayitNo={KayitNo}, Tarih={BaslangicTarihi}-{BitisTarihi}, Kayıt Sayısı={KayitSayisi}",
                    request.TcKimlikNo, kayitNo, request.BaslangicTarihi, request.BitisTarihi, personelData.Count);

                // 3. İzin/Mazeret kayıtlarını al
                var izinMazeretRepo = _unitOfWork.GetRepository<IIzinMazeretTalepRepository>();
                var izinMazeretler = await izinMazeretRepo.GetApprovedByPersonelTcAsync(
                    request.TcKimlikNo,
                    request.BaslangicTarihi,
                    request.BitisTarihi);

                // 4. Günlük mesai kayıtlarını oluştur
                var result = new List<PersonelMesaiListResponseDto>();

                for (var date = request.BaslangicTarihi.Date; date <= request.BitisTarihi.Date; date = date.AddDays(1))
                {
                    var gunlukKayitlar = personelData
                        .Where(x => x.Tarih.HasValue && x.Tarih.Value.Date == date)
                        .ToList();
                    var izinMazeret = izinMazeretler.FirstOrDefault(im =>
                        (im.BaslangicTarihi.HasValue && im.BitisTarihi.HasValue &&
                         date >= im.BaslangicTarihi.Value.Date && date <= im.BitisTarihi.Value.Date) ||
                        (im.MazeretTarihi.HasValue && date == im.MazeretTarihi.Value.Date));

                    var mesaiDto = ProcessGunlukMesai(date, gunlukKayitlar, izinMazeret, personel);
                    result.Add(mesaiDto);
                }

                return ApiResponseDto<List<PersonelMesaiListResponseDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel mesai listesi alınırken hata oluştu: {TcKimlikNo}", request.TcKimlikNo);
                return ApiResponseDto<List<PersonelMesaiListResponseDto>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<PersonelMesaiBaslikDto>> GetPersonelBaslikBilgiAsync(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var personel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);

                if (personel == null)
                {
                    return ApiResponseDto<PersonelMesaiBaslikDto>.ErrorResult($"Personel bulunamadı: {tcKimlikNo}");
                }

                var baslik = new PersonelMesaiBaslikDto
                {
                    AdSoyad = personel.AdSoyad,
                    DepartmanAdi = personel.Departman?.DepartmanAdi ?? "",
                    BirimAdi = personel.Servis?.ServisAdi,
                    SicilNo = personel.SicilNo
                };

                return ApiResponseDto<PersonelMesaiBaslikDto>.SuccessResult(baslik);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel başlık bilgisi alınırken hata oluştu: {TcKimlikNo}", tcKimlikNo);
                return ApiResponseDto<PersonelMesaiBaslikDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        private PersonelMesaiListResponseDto ProcessGunlukMesai(
            DateTime tarih,
            List<CekilenData> gunlukKayitlar,
            IzinMazeretTalep? izinMazeret,
            Personel personel)
        {
            var dto = new PersonelMesaiListResponseDto
            {
                Tarih = tarih,
                TcKimlikNo = personel.TcKimlikNo,
                AdSoyad = personel.AdSoyad,
                SicilNo = personel.SicilNo,
                DepartmanAdi = personel.Departman?.DepartmanAdi ?? "",
                ServisAdi = personel.Servis?.ServisAdi,
                HaftaSonu = IsWeekend(tarih)
            };

            // İzin/Mazeret kontrolü
            if (izinMazeret != null)
            {
                dto.Detay = izinMazeret.Turu.ToString();
                dto.MesaiSuresi = "00:00";
                dto.MesaiSuresiDakika = 0;
                return dto;
            }

            // Kayıt yoksa
            if (!gunlukKayitlar.Any())
            {
                dto.Detay = dto.HaftaSonu ? "Hafta Sonu" : "Kayıt Yok";
                dto.MesaiSuresi = "00:00";
                dto.MesaiSuresiDakika = 0;
                return dto;
            }

            // Giriş/Çıkış eşleştirme
            var girisKayit = gunlukKayitlar
                .Where(x => x.GirisCikisModu == "0") // In
                .OrderBy(x => x.Tarih)
                .FirstOrDefault();

            var cikisKayit = gunlukKayitlar
                .Where(x => x.GirisCikisModu == "1") // Out
                .OrderByDescending(x => x.Tarih)
                .FirstOrDefault();

            if (girisKayit?.Tarih != null)
            {
                dto.GirisSaati = girisKayit.Tarih.Value.TimeOfDay;

                // Geç kalma kontrolü (08:15'ten sonra giriş)
                var normalGirisSaati = new TimeSpan(8, 15, 0);
                dto.GecKalma = dto.GirisSaati > normalGirisSaati;
            }

            if (cikisKayit?.Tarih != null)
            {
                dto.CikisSaati = cikisKayit.Tarih.Value.TimeOfDay;
            }

            // Mesai süresi hesaplama
            if (dto.GirisSaati.HasValue && dto.CikisSaati.HasValue)
            {
                var sure = dto.CikisSaati.Value - dto.GirisSaati.Value;
                if (sure.TotalMinutes > 0)
                {
                    dto.MesaiSuresiDakika = (int)sure.TotalMinutes;
                    dto.MesaiSuresi = $"{(int)sure.TotalHours:D2}:{sure.Minutes:D2}";
                }
                else
                {
                    dto.MesaiSuresi = "00:00";
                    dto.MesaiSuresiDakika = 0;
                }
            }
            else
            {
                dto.MesaiSuresi = "00:00";
                dto.MesaiSuresiDakika = 0;
                dto.Detay = dto.GirisSaati.HasValue ? "Çıkış Kaydı Yok" : "Eksik Kayıt";
            }

            return dto;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
