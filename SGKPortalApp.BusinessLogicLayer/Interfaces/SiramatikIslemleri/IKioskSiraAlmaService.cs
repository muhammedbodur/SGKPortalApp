using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk Sıra Alma Servisi Interface
    /// Masaüstü kiosk uygulamasını simüle eder
    /// 
    /// Vatandaş Akışı:
    /// 1. Kiosk Menüleri → GetKioskMenulerAsync (sadece aktif personeli olan menüler)
    /// 2. Alt Kanal İşlemleri → GetKioskMenuAltIslemleriAsync (seçilen menüdeki işlemler, sadece aktif personeli olanlar)
    /// 3. Sıra Al → SiraAlAsync (seçilen işlem için sıra al)
    /// </summary>
    public interface IKioskSiraAlmaService
    {
        // ═══════════════════════════════════════════════════════
        // YENİ YAPILAR: KIOSK BAZLI İŞLEMLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir Kiosk için menüleri detaylı olarak getirir (YENİ)
        /// Complex query kullanarak kiosk bazlı menü listesini döner
        /// </summary>
        Task<ApiResponseDto<List<KioskMenuDto>>> GetKioskMenulerByKioskIdAsync(int kioskId);

        /// <summary>
        /// Belirli bir Kiosk'taki seçilen menü için alt kanal işlemlerini getirir (YENİ)
        /// Complex query kullanarak kiosk ve menü bazlı alt işlem listesini döner
        /// Sadece aktif personel (Yrd.Uzman+) olan ve banko modunda bulunan işlemler döner
        /// </summary>
        Task<ApiResponseDto<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId);

        // ═══════════════════════════════════════════════════════
        // ESKİ YAPILAR: HİZMET BİNASI BAZLI İŞLEMLER (Geriye Uyumluluk)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// [ESKİ] Hizmet binasındaki kiosk menülerini listeler
        /// Sadece en az bir alt işleminde aktif personel (Yrd.Uzman+) olan menüler döner
        /// </summary>
        Task<ApiResponseDto<List<KioskMenuDto>>> GetKioskMenulerAsync(int hizmetBinasiId);

        /// <summary>
        /// [ESKİ] Seçilen kiosk menüsündeki alt kanal işlemlerini listeler
        /// Sadece aktif personel (Yrd.Uzman+) olan işlemler döner
        /// </summary>
        Task<ApiResponseDto<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriAsync(int hizmetBinasiId, int kioskMenuId);

        // ═══════════════════════════════════════════════════════
        // ADIM 3: SIRA AL
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Kiosk'tan sıra al
        /// - KioskMenuIslemId üzerinden KanalAltId'yi bulur
        /// - HizmetBinasi + KanalAlt kombinasyonundan KanalAltIslem'i bulur
        /// - Bu KanalAltIslem'e atanmış ve banko modunda aktif personel (Yrd.Uzman+) olup olmadığını kontrol eder
        /// - Yeni sıra numarası üretir ve kaydeder
        /// - SignalR ile banko panellerine bildirim gönderir
        /// </summary>
        Task<ApiResponseDto<KioskSiraAlResponseDto>> SiraAlAsync(KioskSiraAlRequestDto request);

        // ═══════════════════════════════════════════════════════
        // YARDIMCI METODLAR
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir hizmet binası ve kanal alt işlem için bekleyen sıra sayısını döner
        /// </summary>
        Task<int> GetBekleyenSiraSayisiAsync(int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Belirli bir hizmet binası ve KanalAltIslem için banko modunda aktif personel (Yrd.Uzman+) var mı?
        /// NOT: kanalAltIslemId parametresi KanalAltIslem tablosundaki ID'dir!
        /// </summary>
        Task<bool> HasAktifPersonelAsync(int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// [DEBUG] Sıra numarası bilgisini test et
        /// </summary>
        Task<object> TestGetSiraNoAsync(int kanalAltIslemId);
    }
}
