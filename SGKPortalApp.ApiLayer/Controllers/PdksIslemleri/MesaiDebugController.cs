using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/mesai-debug")]
    [AllowAnonymous] // Sadece debug için - production'da kaldırılmalı
    public class MesaiDebugController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MesaiDebugController> _logger;

        public MesaiDebugController(
            IUnitOfWork unitOfWork,
            ILogger<MesaiDebugController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Veritabanı bağlantısını ve veri durumunu kontrol et
        /// </summary>
        [HttpGet("check-data")]
        public async Task<IActionResult> CheckData()
        {
            try
            {
                var cekilenDataRepo = _unitOfWork.GetRepository<ICekilenDataRepository>();
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();

                var toplamCekilenData = (await cekilenDataRepo.GetAllAsync()).Count();
                var toplamPersonel = (await personelRepo.GetAllAsync()).Count();

                // Son 10 CekilenData kaydı
                var sonCekilenDatalar = await cekilenDataRepo.GetByDateRangeAsync(
                    DateTime.Now.AddDays(-30),
                    DateTime.Now);

                var result = new
                {
                    Success = true,
                    Message = "Veri kontrolü başarılı",
                    Data = new
                    {
                        ToplamCekilenData = toplamCekilenData,
                        ToplamPersonel = toplamPersonel,
                        Son30GunCekilenData = sonCekilenDatalar.Count(),
                        OrnekCekilenData = sonCekilenDatalar.Take(5).Select(x => new
                        {
                            x.CekilenDataId,
                            x.KayitNo,
                            x.Tarih,
                            x.GirisCikisModu,
                            x.DeviceId
                        }).ToList(),
                        OrnekPersonel = (await personelRepo.GetAllAsync()).Take(5).Select(p => new
                        {
                            p.TcKimlikNo,
                            p.SicilNo,
                            p.AdSoyad,
                            p.PersonelKayitNo,
                            p.DepartmanId
                        }).ToList()
                    }
                };

                _logger.LogInformation(
                    "Debug check: CekilenData={CekilenData}, Personel={Personel}",
                    toplamCekilenData,
                    toplamPersonel);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Debug check hatası");
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Hata: {ex.Message}",
                    StackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// KayitNo bazlı veri kontrolü - Hangi KayitNo'lar var?
        /// </summary>
        [HttpGet("check-kayitno")]
        public async Task<IActionResult> CheckKayitNo()
        {
            try
            {
                var cekilenDataRepo = _unitOfWork.GetRepository<ICekilenDataRepository>();
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();

                var tumCekilenData = await cekilenDataRepo.GetAllAsync();
                var tumPersonel = await personelRepo.GetAllAsync();

                // Unique KayitNo'ları bul
                var uniqueKayitNolar = tumCekilenData
                    .Where(x => !string.IsNullOrEmpty(x.KayitNo))
                    .Select(x => x.KayitNo)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                // Her KayitNo için personel eşleşmesi kontrol et
                var kayitNoAnaliz = uniqueKayitNolar.Select(kayitNo =>
                {
                    var personel = tumPersonel.FirstOrDefault(p => p.PersonelKayitNo.ToString() == kayitNo);
                    var kayitSayisi = tumCekilenData.Count(x => x.KayitNo == kayitNo);

                    return new
                    {
                        KayitNo = kayitNo,
                        PersonelBulundu = personel != null,
                        PersonelAdSoyad = personel?.AdSoyad,
                        PersonelTcKimlikNo = personel?.TcKimlikNo,
                        PersonelSicilNo = personel?.SicilNo,
                        CekilenDataKayitSayisi = kayitSayisi
                    };
                }).ToList();

                var result = new
                {
                    Success = true,
                    ToplamUniqueKayitNo = uniqueKayitNolar.Count,
                    PersonelEslesmeyenKayitNo = kayitNoAnaliz.Count(x => !x.PersonelBulundu),
                    PersonelEslesenKayitNo = kayitNoAnaliz.Count(x => x.PersonelBulundu),
                    KayitNoDetay = kayitNoAnaliz
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "KayitNo check hatası");
                return BadRequest(new { Success = false, Message = $"Hata: {ex.Message}" });
            }
        }

        /// <summary>
        /// Belirli bir personelin mesai verilerini kontrol et
        /// </summary>
        [HttpGet("check-personel/{tcKimlikNo}")]
        public async Task<IActionResult> CheckPersonel(string tcKimlikNo)
        {
            try
            {
                var personelRepo = _unitOfWork.GetRepository<IPersonelRepository>();
                var cekilenDataRepo = _unitOfWork.GetRepository<ICekilenDataRepository>();

                var personel = await personelRepo.GetByTcKimlikNoWithDetailsAsync(tcKimlikNo);
                if (personel == null)
                {
                    return NotFound(new { Message = $"Personel bulunamadı: {tcKimlikNo}" });
                }

                var kayitNo = personel.PersonelKayitNo.ToString();
                var tumCekilenData = await cekilenDataRepo.GetByDateRangeAsync(
                    DateTime.Now.AddDays(-30),
                    DateTime.Now);

                var personelCekilenData = tumCekilenData
                    .Where(x => !string.IsNullOrEmpty(x.KayitNo) && x.KayitNo == kayitNo)
                    .OrderByDescending(x => x.Tarih)
                    .Take(20)
                    .ToList();

                var result = new
                {
                    Success = true,
                    Personel = new
                    {
                        personel.TcKimlikNo,
                        personel.SicilNo,
                        personel.AdSoyad,
                        personel.PersonelKayitNo,
                        KayitNoString = kayitNo,
                        DepartmanAdi = personel.Departman?.DepartmanAdi,
                        ServisAdi = personel.Servis?.ServisAdi
                    },
                    CekilenDataSayisi = personelCekilenData.Count,
                    Son20Kayit = personelCekilenData.Select(x => new
                    {
                        x.CekilenDataId,
                        x.KayitNo,
                        Tarih = x.Tarih?.ToString("dd.MM.yyyy HH:mm:ss"),
                        x.GirisCikisModu,
                        GirisCikisModuAciklama = x.GirisCikisModu == "0" ? "Giriş" : 
                                                 x.GirisCikisModu == "1" ? "Çıkış" : "Diğer",
                        x.DeviceId
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel check hatası: {TcKimlikNo}", tcKimlikNo);
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Hata: {ex.Message}"
                });
            }
        }
    }
}
