using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class HizmetBinasiResponseDto
    {
        public int HizmetBinasiId { get; set; }
        public required string HizmetBinasiAdi { get; set; }
        public int? LegacyKod { get; set; }
        public string Adres { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        
        /// <summary>
        /// Bu binada bulunan departmanlar (many-to-many)
        /// </summary>
        public List<DepartmanBinaDto> Departmanlar { get; set; } = new();
        
        /// <summary>
        /// Geriye uyumluluk için - ilk departman adı (varsa)
        /// </summary>
        public string DepartmanAdi => Departmanlar.FirstOrDefault()?.DepartmanAdi ?? string.Empty;
        
        public int PersonelSayisi { get; set; }
        public int BankoSayisi { get; set; }
        public int TvSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }

    /// <summary>
    /// Bina-Departman ilişkisi için basit DTO
    /// </summary>
    public class DepartmanBinaDto
    {
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public bool AnaBina { get; set; }
    }
}