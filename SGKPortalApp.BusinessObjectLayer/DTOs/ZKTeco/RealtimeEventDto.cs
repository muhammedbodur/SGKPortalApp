using System;

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
        /// 0 = Password, 1 = Fingerprint, 15 = Card, 3 = Face
        /// </summary>
        public int VerifyMethod { get; set; }

        /// <summary>
        /// Giriş/Çıkış durumu
        /// 0 = CheckIn, 1 = CheckOut, 2 = BreakOut, 3 = BreakIn, 4 = OTIn, 5 = OTOut
        /// </summary>
        public int InOutMode { get; set; }

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
    }
}
