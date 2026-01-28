using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelUpdateRequestDto
    {
        public string? RequestorTcKimlikNo { get; set; }
        public string? RequestorSessionId { get; set; }

        [Required]
        [StringLength(200)]
        public string AdSoyad { get; set; } = string.Empty;

        [StringLength(50)]
        public string? NickName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public int DepartmanId { get; set; }
        public int ServisId { get; set; }
        public int UnvanId { get; set; }
        public int DepartmanHizmetBinasiId { get; set; }

        public int Dahili { get; set; }

        [StringLength(20)]
        public string? CepTelefonu { get; set; }

        [StringLength(20)]
        public string? CepTelefonu2 { get; set; }

        [StringLength(20)]
        public string? EvTelefonu { get; set; }

        [StringLength(500)]
        public string? Adres { get; set; }

        [StringLength(100)]
        public string? Semt { get; set; }

        public MedeniDurumu MedeniDurumu { get; set; }
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; }

        /// <summary>
        /// Pasif/ayrılış nedeni açıklaması
        /// </summary>
        [StringLength(500)]
        public string? PasifNedeni { get; set; }

        /// <summary>
        /// Durum değişiklik tarihi (pasif/emekli/istifa vb. olduğu tarih)
        /// </summary>
        public DateTime? DurumDegisiklikTarihi { get; set; }

        // Acil Durum Yakını Bilgileri
        [StringLength(100)]
        public string? YakinAdSoyad { get; set; }

        [StringLength(50)]
        public string? YakinlikDerecesi { get; set; }

        [StringLength(20)]
        public string? YakinTelefonu { get; set; }

        // Kadro ve Göreve Başlama Bilgileri
        [StringLength(50)]
        public string? KadroDerecesi { get; set; }

        public DateTime? GoreveBaslamaTarihi { get; set; }

        // Finansal Bilgiler
        [StringLength(34)]
        public string? Iban { get; set; }

        // Ulaşım Bilgileri
        [StringLength(20)]
        public string? EshotHat { get; set; }

        [StringLength(50)]
        public string? Durak { get; set; }

        // Taşeron Bilgisi
        [StringLength(100)]
        public string? TaseronFirma { get; set; }

        [StringLength(255)]
        public string? Resim { get; set; }

        // PDKS Alanları
        public int UlasimServis1 { get; set; }
        public int UlasimServis2 { get; set; }
        public int Tabldot { get; set; }
        public int PersonelKayitNo { get; set; }
        public int KartNo { get; set; }
        public DateTime? KartNoAktiflikTarihi { get; set; }
    }
}