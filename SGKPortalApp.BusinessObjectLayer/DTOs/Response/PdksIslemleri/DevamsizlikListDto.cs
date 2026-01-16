using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Devamsızlık/Mazeret Liste Response DTO
    /// </summary>
    public class DevamsizlikListDto
    {
        public int Id { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public IzinMazeretTuru Turu { get; set; }
        public string TuruAdi { get; set; } = string.Empty;
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public DateTime? MazeretTarihi { get; set; }
        public SaatDilimi? SaatDilimi { get; set; }
        public string? SaatDilimiAdi { get; set; }
        public string? Aciklama { get; set; }
        public bool OnayDurumu { get; set; }
        public int? OnaylayanSicilNo { get; set; }
        public string? OnaylayanAdSoyad { get; set; }
        public DateTime? OnayTarihi { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
