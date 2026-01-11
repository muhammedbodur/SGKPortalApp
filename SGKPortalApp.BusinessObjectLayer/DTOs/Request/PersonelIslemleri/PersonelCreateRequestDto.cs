using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelCreateRequestDto
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sicil No zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Sicil No giriniz")]
        public int SicilNo { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(200, ErrorMessage = "Ad Soyad en fazla 200 karakter olabilir")]
        public string AdSoyad { get; set; } = string.Empty;

        [StringLength(50)]
        public string? NickName { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Organizasyon
        [Required(ErrorMessage = "Departman seçimi zorunludur")]
        public int DepartmanId { get; set; }

        [Required(ErrorMessage = "Servis seçimi zorunludur")]
        public int ServisId { get; set; }

        [Required(ErrorMessage = "Unvan seçimi zorunludur")]
        public int UnvanId { get; set; }

        [Required]
        public int AtanmaNedeniId { get; set; }

        [Required]
        public int HizmetBinasiId { get; set; }

        [Required]
        public int IlId { get; set; }

        [Required]
        public int IlceId { get; set; }

        // ⭐ DEĞİŞTİRİLDİ: Nullable yapıldı
        public int? SendikaId { get; set; }

        // İletişim
        public int Dahili { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
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

        // Kişisel Bilgiler
        [Required(ErrorMessage = "Doğum tarihi zorunludur")]
        public DateTime DogumTarihi { get; set; }

        [Required]
        public Cinsiyet Cinsiyet { get; set; }

        [Required]
        public MedeniDurumu MedeniDurumu { get; set; }

        [Required]
        public KanGrubu KanGrubu { get; set; }

        public EvDurumu EvDurumu { get; set; }

        public int UlasimServis1 { get; set; }

        public int UlasimServis2 { get; set; }

        public int Tabldot { get; set; }

        public int PersonelKayitNo { get; set; }

        public int KartNo { get; set; }

        public DateTime? KartNoAktiflikTarihi { get; set; }

        [Required]
        public PersonelTipi PersonelTipi { get; set; }

        [Required]
        public OgrenimDurumu OgrenimDurumu { get; set; }

        [StringLength(200)]
        public string? BitirdigiOkul { get; set; }

        [StringLength(100)]
        public string? BitirdigiBolum { get; set; }

        public int OgrenimSuresi { get; set; }

        [StringLength(255)]
        public string? Resim { get; set; }

        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; } = PersonelAktiflikDurum.Aktif;

        // Eş Bilgileri
        [StringLength(200)]
        public string? EsininAdi { get; set; }

        public EsininIsDurumu EsininIsDurumu { get; set; }

        [StringLength(100)]
        public string? EsininUnvani { get; set; }

        [StringLength(500)]
        public string? EsininIsAdresi { get; set; }

        public int? EsininIsIlId { get; set; }

        public int? EsininIsIlceId { get; set; }

        [StringLength(100)]
        public string? EsininIsSemt { get; set; }

        // Dinamik Listeler
        public List<PersonelCocukCreateDto>? Cocuklar { get; set; }
        public List<PersonelHizmetCreateDto>? Hizmetler { get; set; }
        public List<PersonelEgitimCreateDto>? Egitimler { get; set; }
        public List<PersonelYetkiCreateDto>? Yetkiler { get; set; }
        public List<PersonelCezaCreateDto>? Cezalar { get; set; }
        public List<PersonelEngelCreateDto>? Engeller { get; set; }
    }

    // Nested DTO'lar
    public class PersonelCocukCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Isim { get; set; } = string.Empty;

        public DateTime? DogumTarihi { get; set; }

        [StringLength(100)]
        public string? Egitim { get; set; }
    }

    public class PersonelHizmetCreateDto
    {
        [StringLength(200)]
        public string? Departman { get; set; }

        [StringLength(200)]
        public string? Servis { get; set; }

        public DateTime? BaslamaTarihi { get; set; }

        public DateTime? AyrilmaTarihi { get; set; }

        [StringLength(500)]
        public string? Sebep { get; set; }
    }

    public class PersonelEgitimCreateDto
    {
        [StringLength(200)]
        public string? EgitimAdi { get; set; }

        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }

    public class PersonelYetkiCreateDto
    {
        public int DepartmanId { get; set; }

        public int ServisId { get; set; }

        [StringLength(500)]
        public string? Sebep { get; set; }

        public DateTime? BaslamaTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }

    public class PersonelCezaCreateDto
    {
        [StringLength(500)]
        public string? Sebep { get; set; }

        [StringLength(200)]
        public string? AltBendi { get; set; }

        public DateTime? CezaTarihi { get; set; }
    }

    public class PersonelEngelCreateDto
    {
        public EngelDerecesi EngelDerecesi { get; set; }

        [StringLength(500)]
        public string? Neden1 { get; set; }

        [StringLength(500)]
        public string? Neden2 { get; set; }

        [StringLength(500)]
        public string? Neden3 { get; set; }
    }
}