using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Detay sayfası için - Personel ve Banko listesi ile birlikte
    /// </summary>
    public class HizmetBinasiDetailResponseDto
    {
        public int HizmetBinasiId { get; set; }
        public required string HizmetBinasiAdi { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public Aktiflik HizmetBinasiAktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }

        // İlişkisel veriler
        public List<PersonelResponseDto> Personeller { get; set; } = new();
        public List<BankoResponseDto> Bankolar { get; set; } = new();
        public List<TvResponseDto> Tvler { get; set; } = new();

        // İstatistikler
        public int ToplamPersonelSayisi => Personeller.Count;
        public int AktifPersonelSayisi => Personeller.Count(p => p.PersonelAktiflikDurum == PersonelIslemleri.PersonelAktiflikDurum.Aktif);
        public int ToplamBankoSayisi => Bankolar.Count;
        public int AktifBankoSayisi => Bankolar.Count(b => b.BankoAktiflik == Aktiflik.Aktif);
    }
}