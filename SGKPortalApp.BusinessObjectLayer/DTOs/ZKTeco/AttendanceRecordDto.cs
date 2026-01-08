using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco attendance (giriş-çıkış) kayıt DTO
    /// ZKTecoApi GET /api/attendance/{ip} yanıtı
    /// </summary>
    public class AttendanceRecordDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }
        public int VerifyMethod { get; set; }
        public int InOutMode { get; set; }
        public int WorkCode { get; set; }
        public string DeviceIp { get; set; } = string.Empty;
        public long? CardNumber { get; set; }

        /// <summary>
        /// Doğrulama yöntemi metni
        /// 0=Password, 1=Fingerprint, 15=Card, 25=Face
        /// </summary>
        public string VerifyMethodText => VerifyMethod switch
        {
            0 => "Şifre",
            1 => "Parmak İzi",
            15 => "Kart",
            25 => "Yüz Tanıma",
            _ => $"Bilinmiyor ({VerifyMethod})"
        };

        /// <summary>
        /// Giriş-çıkış modu metni
        /// 0=CheckIn, 1=CheckOut, 2=BreakOut, 3=BreakIn, 4=OTIn, 5=OTOut
        /// </summary>
        public string InOutModeText => InOutMode switch
        {
            0 => "Giriş",
            1 => "Çıkış",
            2 => "Mola Başlangıç",
            3 => "Mola Bitiş",
            4 => "Mesai Giriş",
            5 => "Mesai Çıkış",
            _ => $"Bilinmiyor ({InOutMode})"
        };

        /// <summary>
        /// Tarih string formatı
        /// </summary>
        public string EventDateText => EventTime.ToString("dd.MM.yyyy");

        /// <summary>
        /// Saat string formatı
        /// </summary>
        public string EventTimeText => EventTime.ToString("HH:mm:ss");
    }
}
