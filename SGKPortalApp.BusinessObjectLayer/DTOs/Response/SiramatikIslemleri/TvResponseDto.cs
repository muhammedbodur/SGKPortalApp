using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// TV ekran bilgilerini döndürmek için kullanılan DTO
    /// </summary>
    public class TvResponseDto
    {
        public int TvId { get; set; }
        public string TvAdi { get; set; } = string.Empty;
        public string? TvAciklama { get; set; }
        public KatTipi KatTipi { get; set; }
        public Aktiflik Aktiflik { get; set; }

        // Departman-Hizmet Binası kombinasyonu
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string? HizmetBinasiAdi { get; set; }

        // İlişkili Banko Sayısı
        public int BankoSayisi { get; set; }

        // Eşleşmiş Banko ID'leri (BankoEslestirme için)
        public List<int>? EslesmiBankoIdler { get; set; }

        // Bağlantı Durumu
        public bool IsConnected { get; set; }

        // Audit Fields
        public DateTime EklenmeTarihi { get; set; }
        public DateTime? DuzenlenmeTarihi { get; set; }
    }
}