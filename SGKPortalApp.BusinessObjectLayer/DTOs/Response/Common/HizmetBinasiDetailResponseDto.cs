using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Hizmet Binası detay bilgilerini döndürmek için kullanılan DTO
    /// İlişkili tüm personel, banko ve TV bilgilerini içerir
    /// </summary>
    public class HizmetBinasiDetailResponseDto
    {
        // ═══════════════════════════════════════════════════════
        // TEMEL BİLGİLER
        // ═══════════════════════════════════════════════════════

        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public int? LegacyKod { get; set; }
        public string? Adres { get; set; } 
        public Aktiflik Aktiflik { get; set; }
        
        /// <summary>
        /// Bu binada bulunan departmanlar (many-to-many)
        /// </summary>
        public List<DepartmanBinaDto> Departmanlar { get; set; } = new();
        
        /// <summary>
        /// Geriye uyumluluk için - ilk departman adı (varsa)
        /// </summary>
        public string DepartmanAdi => Departmanlar.FirstOrDefault()?.DepartmanAdi ?? string.Empty;
        
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }

        // ═══════════════════════════════════════════════════════
        // İSTATİSTİKLER
        // ═══════════════════════════════════════════════════════

        public int ToplamPersonelSayisi { get; set; }
        public int AktifPersonelSayisi { get; set; } // ✅ EKLEME
        public int ToplamBankoSayisi { get; set; }
        public int AktifBankoSayisi { get; set; } // ✅ EKLEME
        public int ToplamTvSayisi { get; set; }

        // ═══════════════════════════════════════════════════════
        // İLİŞKİLİ VERİLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Hizmet binasına bağlı tüm bankolar
        /// </summary>
        public List<BankoResponseDto> Bankolar { get; set; } = new();

        /// <summary>
        /// Hizmet binasına bağlı tüm TV ekranları
        /// </summary>
        public List<TvResponseDto> Tvler { get; set; } = new();

        /// <summary>
        /// Hizmet binasına bağlı tüm personeller
        /// </summary>
        public List<PersonelResponseDto> Personeller { get; set; } = new();

        /// <summary>
        /// Personel durumuna göre istatistikler
        /// Key: PersonelAktiflikDurum, Value: Personel Sayısı
        /// </summary>
        public Dictionary<PersonelAktiflikDurum, int> PersonelDurumIstatistikleri { get; set; } = new();
    }
}