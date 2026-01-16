using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// Devamsızlık/Mazeret Filtre Request DTO
    /// </summary>
    public class DevamsizlikFilterDto
    {
        public string? TcKimlikNo { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public bool? SadeceOnayBekleyenler { get; set; }
    }
}
