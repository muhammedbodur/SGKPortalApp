using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    public class CardMismatchDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long? CardNumber { get; set; }
        public string MismatchType { get; set; } = string.Empty; // "OnlyInDevice", "OnlyInDatabase", "DataMismatch"
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceIp { get; set; } = string.Empty;
        public int DeviceId { get; set; }
        
        // Cihazdan gelen bilgiler
        public string? DeviceUserName { get; set; }
        public long? DeviceCardNumber { get; set; }
        
        // Veritabanından gelen bilgiler
        public string? DatabaseUserName { get; set; }
        public long? DatabaseCardNumber { get; set; }
        public string? UserType { get; set; } // "Personel" veya "SpecialCard"
        public int? UserId { get; set; }
        
        public string Details { get; set; } = string.Empty;
        public DateTime CheckedAt { get; set; } = DateTime.Now;
    }
}
