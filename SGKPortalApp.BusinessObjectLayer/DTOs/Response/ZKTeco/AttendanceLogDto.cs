using System;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco
{
    /// <summary>
    /// ZKTeco attendance log
    /// ZKTecoApi → AttendanceLogResponse mapping
    /// </summary>
    public class AttendanceLogDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public VerifyMethod VerifyMethod { get; set; }
        public InOutMode InOutMode { get; set; }
        public int WorkCode { get; set; }
        public string DeviceIp { get; set; } = string.Empty;
        
        // Personel bilgileri (opsiyonel - eşleştirme yapıldıysa dolu)
        public string? PersonelAdSoyad { get; set; }
        public string? PersonelSicilNo { get; set; }
        public string? PersonelDepartman { get; set; }
        public string? PersonelServis { get; set; }
        public string? PersonelTcKimlikNo { get; set; }
    }
}
