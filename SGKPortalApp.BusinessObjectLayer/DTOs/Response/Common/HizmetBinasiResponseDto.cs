using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class HizmetBinasiResponseDto
    {
        public int HizmetBinasiId { get; set; }
        public required string HizmetBinasiAdi { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public int PersonelSayisi { get; set; }
        public int BankoSayisi { get; set; }
        public int TvSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}