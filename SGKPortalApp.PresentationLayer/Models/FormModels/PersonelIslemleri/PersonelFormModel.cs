using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    public class PersonelFormModel
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        public int? SicilNo { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Ad Soyad 3-200 karakter arasında olmalıdır")]
        public string AdSoyad { get; set; } = string.Empty;

        public string? NickName { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string? Email { get; set; }

        // ═══════════════════════════════════════════════════════
        // KURUM BİLGİLERİ
        // ═══════════════════════════════════════════════════════
        [Required(ErrorMessage = "Departman seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir departman seçiniz")]
        public int DepartmanId { get; set; }

        [Required(ErrorMessage = "Servis seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir servis seçiniz")]
        public int ServisId { get; set; }

        [Required(ErrorMessage = "Ünvan seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir ünvan seçiniz")]
        public int UnvanId { get; set; }

        /// <summary>
        /// Departman-HizmetBinası kombinasyonu (junction table ID)
        /// Departman seçildikten sonra o departmana bağlı binalar listelenir
        /// </summary>
        [Required(ErrorMessage = "Hizmet binası seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir hizmet binası seçiniz")]
        public int DepartmanHizmetBinasiId { get; set; }

        public int? AtanmaNedeniId { get; set; }
        public int? SendikaId { get; set; }

        // ═══════════════════════════════════════════════════════
        // İLETİŞİM BİLGİLERİ
        // ═══════════════════════════════════════════════════════
        public int Dahili { get; set; }
        public string? CepTelefonu { get; set; }
        public string? CepTelefonu2 { get; set; }
        public string? EvTelefonu { get; set; }
        public string? Adres { get; set; }
        public string? Semt { get; set; }

        [Required(ErrorMessage = "İl seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir il seçiniz")]
        public int IlId { get; set; }

        [Required(ErrorMessage = "İlçe seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir ilçe seçiniz")]
        public int IlceId { get; set; }

        // ═══════════════════════════════════════════════════════
        // KİŞİSEL BİLGİLER
        // ═══════════════════════════════════════════════════════
        public DateTime DogumTarihi { get; set; } = DateTime.Now.AddYears(-30);
        public Cinsiyet Cinsiyet { get; set; }
        public MedeniDurumu MedeniDurumu { get; set; }
        public KanGrubu KanGrubu { get; set; }
        public EvDurumu EvDurumu { get; set; }
        public int UlasimServis1 { get; set; } = 0;
        public int UlasimServis2 { get; set; } = 0;
        public int Tabldot { get; set; } = 0;
        public int PersonelKayitNo { get; set; } = 0;
        public int KartNo { get; set; } = 0;
        public DateTime? KartNoAktiflikTarihi { get; set; }
        public DateTime? KartNoDuzenlenmeTarihi { get; set; }
        public DateTime? KartNoGonderimTarihi { get; set; }
        public IslemBasari KartGonderimIslemBasari { get; set; }
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; } = PersonelAktiflikDurum.Aktif;
        public PersonelTipi PersonelTipi { get; set; }

        /// <summary>
        /// Pasif/ayrılış nedeni açıklaması
        /// </summary>
        public string? PasifNedeni { get; set; }

        /// <summary>
        /// Durum değişiklik tarihi (pasif/emekli/istifa vb. olduğu tarih)
        /// </summary>
        public DateTime? DurumDegisiklikTarihi { get; set; }

        // ═══════════════════════════════════════════════════════
        // ACİL DURUM YAKINI BİLGİLERİ
        // ═══════════════════════════════════════════════════════
        public string? YakinAdSoyad { get; set; }
        public string? YakinlikDerecesi { get; set; }
        public string? YakinTelefonu { get; set; }

        // ═══════════════════════════════════════════════════════
        // KADRO VE GÖREVE BAŞLAMA BİLGİLERİ
        // ═══════════════════════════════════════════════════════
        public string? KadroDerecesi { get; set; }
        public DateTime? GoreveBaslamaTarihi { get; set; }

        // ═══════════════════════════════════════════════════════
        // FİNANSAL BİLGİLER
        // ═══════════════════════════════════════════════════════
        public string? Iban { get; set; }

        // ═══════════════════════════════════════════════════════
        // ULAŞIM BİLGİLERİ
        // ═══════════════════════════════════════════════════════
        public string? EshotHat { get; set; }
        public string? Durak { get; set; }

        // ═══════════════════════════════════════════════════════
        // TAŞERON BİLGİSİ
        // ═══════════════════════════════════════════════════════
        public string? TaseronFirma { get; set; }

        // ═══════════════════════════════════════════════════════
        // EĞİTİM BİLGİLERİ
        // ═══════════════════════════════════════════════════════
        public OgrenimDurumu OgrenimDurumu { get; set; }
        public string? BitirdigiOkul { get; set; }
        public string? BitirdigiBolum { get; set; }

        // ✅ EKSİK PROPERTY EKLENDİ
        public int OgrenimSuresi { get; set; } = 0;
        public string? Bransi { get; set; }

        public string? EmekliSicilNo { get; set; }
        public SehitYakinligi SehitYakinligi { get; set; }

        // ═════════════════════════════════════════════════════
        // DİĞER BİLGİLER
        // ═══════════════════════════════════════════════════════
        public string? Resim { get; set; }
        public string? EsininAdi { get; set; }
        public EsininIsDurumu EsininIsDurumu { get; set; }
        public string? EsininUnvani { get; set; }
        public string? EsininIsAdresi { get; set; }
    }
}