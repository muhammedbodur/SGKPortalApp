using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Departman Mesai Raporu Response DTO
    /// Departman seviyesinde toplu mesai raporu
    /// </summary>
    public class DepartmanMesaiReportDto
    {
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? ServisAdi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public List<PersonelMesaiOzetDto> Personeller { get; set; } = new();
    }
}
