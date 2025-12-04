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
        // ADIM 1: KIOSK MENÜLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Hizmet binasındaki kiosk menülerini listeler
        /// Sadece en az bir alt işleminde aktif personel (Yrd.Uzman+) olan menüler döner
        /// </summary>
        Task<ApiResponseDto<List<KioskMenuDto>>> GetKioskMenulerAsync(int hizmetBinasiId);

        // ═══════════════════════════════════════════════════════
        // ADIM 2: ALT KANAL İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Seçilen kiosk menüsündeki alt kanal işlemlerini listeler
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
        /// Belirli bir hizmet binası ve KanalAlt için banko modunda aktif personel (Yrd.Uzman+) var mı?
        /// NOT: kanalAltId parametresi KanalAlt tablosundaki ID'dir (KanalAltIslem değil!)
        /// </summary>
        Task<bool> HasAktifPersonelAsync(int hizmetBinasiId, int kanalAltId);
    }
}
