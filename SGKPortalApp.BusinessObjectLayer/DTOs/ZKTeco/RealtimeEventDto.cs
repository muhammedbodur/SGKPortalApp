using System;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTecoApi realtime event response
    /// SignalR Hub'dan gelen event data
    /// ZKTecoApi → RealtimeEventResponse mapping
    /// </summary>
    public class RealtimeEventDto
    {
        /// <summary>
        /// Kullanıcı enrollment numarası (Personel sicil no)
        /// </summary>
        public string EnrollNumber { get; set; } = string.Empty;

        /// <summary>
        /// Event zamanı
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Doğrulama yöntemi
        /// </summary>
        public VerifyMethod VerifyMethod { get; set; }

        /// <summary>
        /// Giriş/Çıkış durumu
        /// </summary>
        public InOutMode InOutMode { get; set; }

        /// <summary>
        /// İş kodu (Work code)
        /// </summary>
        public int WorkCode { get; set; }

        /// <summary>
        /// Event'in geldiği cihaz IP adresi
        /// </summary>
        public string DeviceIp { get; set; } = string.Empty;

        /// <summary>
        /// Event geçerli mi? (SDK'dan gelen flag)
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Kart numarası (varsa)
        /// </summary>
        public long? CardNumber { get; set; }
        
        // ═══════════════════════════════════════════════════════
        // PERSONEL BİLGİLERİ (Business katmanı tarafından eklenir)
        // ═══════════════════════════════════════════════════════
        
        /// <summary>
        /// Personel ad soyad (DB'den eşleştirme sonucu)
        /// </summary>
        public string? PersonelAdSoyad { get; set; }
        
        /// <summary>
        /// Personel sicil numarası (DB'den eşleştirme sonucu)
        /// </summary>
        public string? PersonelSicilNo { get; set; }
        
        /// <summary>
        /// Personel departman adı (DB'den eşleştirme sonucu)
        /// </summary>
        public string? PersonelDepartman { get; set; }
        
        /// <summary>
        /// Personel TC kimlik numarası (DB'den eşleştirme sonucu)
        /// </summary>
        public string? PersonelTcKimlikNo { get; set; }
    }
}
