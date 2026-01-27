using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Attributes;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    /// <summary>
    /// Personel entity - Audit logging aktif
    /// Smart Hybrid: Küçük değişiklikler DB'de, büyük değişiklikler dosyada
    /// Transaction grouping: CreateComplete/UpdateComplete işlemlerinde tüm değişiklikler tek log
    /// </summary>
    [AuditLog(
        Insert = true,
        Update = true,
        Delete = true,
        StorageStrategy = StorageStrategy.SmartHybrid,
        HybridThresholdBytes = 1024,
        BulkThreshold = 50,
        GroupRelatedChanges = true)]
    public class Personel : AuditableEntity
    {
        [Key]
        [StringLength(11)]
        [SensitiveData]
        public string TcKimlikNo { get; set; } = string.Empty;

        public int? SicilNo { get; set; }

        [Required]
        [StringLength(200)]
        public string AdSoyad { get; set; } = string.Empty;

        /// <summary>
        /// NickName - Cihazda görünecek kısa isim
        /// Otomatik oluşturulur: Max 8 karakter, büyük harf, Türkçe karakter yok
        /// </summary>
        [Required]
        [StringLength(8)]
        public string NickName { get; set; } = string.Empty;

        public int? PersonelKayitNo { get; set; }
        public int KartNo { get; set; }
        public DateTime? KartNoAktiflikTarihi { get; set; }
        public DateTime? KartNoDuzenlenmeTarihi { get; set; }
        public DateTime? KartNoGonderimTarihi { get; set; }
        public IslemBasari KartGonderimIslemBasari { get; set; }

        /// <summary>
        /// Kart tipi - Personel kartı veya geçici kart
        /// </summary>
        public CardType KartTipi { get; set; } = CardType.PersonelKarti;

        [Required]
        public int DepartmanId { get; set; }
        [ForeignKey(nameof(DepartmanId))]
        [InverseProperty("Personeller")]
        public Departman? Departman { get; set; }

        public int ServisId { get; set; }
        [ForeignKey(nameof(ServisId))]
        [InverseProperty("Personeller")]
        public Servis? Servis { get; set; }

        public int UnvanId { get; set; }
        [ForeignKey(nameof(UnvanId))]
        [InverseProperty("Personeller")]
        public Unvan? Unvan { get; set; }

        public int AtanmaNedeniId { get; set; }
        [ForeignKey(nameof(AtanmaNedeniId))]
        [InverseProperty("Personeller")]
        public AtanmaNedenleri? AtanmaNedeni { get; set; }

        public int DepartmanHizmetBinasiId { get; set; }
        [ForeignKey(nameof(DepartmanHizmetBinasiId))]
        [InverseProperty("Personeller")]
        public required DepartmanHizmetBinasi DepartmanHizmetBinasi { get; set; }

        public int IlId { get; set; }
        [ForeignKey(nameof(IlId))]
        public Il? Il { get; set; }

        public int IlceId { get; set; }
        [ForeignKey(nameof(IlceId))]
        public Ilce? Ilce { get; set; }

        public int? SendikaId { get; set; }
        [ForeignKey(nameof(SendikaId))]
        [InverseProperty("Personeller")]
        public Sendika? Sendika { get; set; }

        // Eşinin iş bilgileri
        public int? EsininIsIlId { get; set; }
        [ForeignKey(nameof(EsininIsIlId))]
        public Il? EsininIsIl { get; set; }

        public int? EsininIsIlceId { get; set; }
        [ForeignKey(nameof(EsininIsIlceId))]
        public Ilce? EsininIsIlce { get; set; }

        // Personal Information
        [StringLength(100)]
        public string? Gorev { get; set; }

        [StringLength(100)]
        public string? Uzmanlik { get; set; }

        public PersonelTipi PersonelTipi { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public int Dahili { get; set; }

        [StringLength(20)]
        [SensitiveData]
        public string? CepTelefonu { get; set; }

        [StringLength(20)]
        [SensitiveData]
        public string? CepTelefonu2 { get; set; }

        [StringLength(20)]
        [SensitiveData]
        public string? EvTelefonu { get; set; }

        [StringLength(500)]
        [SensitiveData]
        public string? Adres { get; set; }

        [StringLength(100)]
        public string? Semt { get; set; }

        public DateTime DogumTarihi { get; set; }

        [StringLength(200)]
        public string? DogumYeri { get; set; }  // PDKS.Net4.8 personelBirthplace uyumluluğu

        public Cinsiyet Cinsiyet { get; set; }
        public MedeniDurumu MedeniDurumu { get; set; }
        public KanGrubu KanGrubu { get; set; }
        public EvDurumu EvDurumu { get; set; }
        public int UlasimServis1 { get; set; } = 0;
        public int UlasimServis2 { get; set; } = 0;
        public int Tabldot { get; set; } = 0;
        
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
        [SensitiveData]
        public string? YakinTelefonu { get; set; }

        // Kadro ve Göreve Başlama Bilgileri
        [StringLength(50)]
        public string? KadroDerecesi { get; set; }

        public DateTime? GoreveBaslamaTarihi { get; set; }

        // Finansal Bilgiler
        [StringLength(34)]
        [SensitiveData]
        public string? Iban { get; set; }

        // Ulaşım Bilgileri
        [StringLength(20)]
        public string? EshotHat { get; set; }

        [StringLength(50)]
        public string? Durak { get; set; }

        // Taşeron Bilgisi
        [StringLength(100)]
        public string? TaseronFirma { get; set; }

        // PDKS.Net4.8 mesai saatleri uyumluluğu (mesaiBaslar, mesaiBiter)
        public TimeSpan? MesaiBaslangicSaati { get; set; }
        public TimeSpan? MesaiBitisSaati { get; set; }

        [StringLength(20)]
        public string? EmekliSicilNo { get; set; }

        public OgrenimDurumu OgrenimDurumu { get; set; }

        [StringLength(200)]
        public string? BitirdigiOkul { get; set; }

        [StringLength(100)]
        public string? BitirdigiBolum { get; set; }

        public int OgrenimSuresi { get; set; } = 0;

        [StringLength(100)]
        public string? Bransi { get; set; }

        public SehitYakinligi SehitYakinligi { get; set; }

        [StringLength(100)]
        public string? EsininAdi { get; set; }

        public EsininIsDurumu EsininIsDurumu { get; set; } = EsininIsDurumu.belirtilmemis;

        [StringLength(100)]
        public string? EsininUnvani { get; set; }

        [StringLength(200)]
        public string? EsininIsAdresi { get; set; }

        [StringLength(100)]
        public string? EsininIsSemt { get; set; }

        public string? HizmetBilgisi { get; set; }
        public string? EgitimBilgisi { get; set; }
        public string? ImzaYetkileri { get; set; }
        public string? CezaBilgileri { get; set; }
        public string? EngelBilgileri { get; set; }

        [StringLength(255)]
        public string? Resim { get; set; }

        // User ile One-to-One ilişki (Dinamik veriler User tablosunda)
        [InverseProperty("Personel")]
        public User? User { get; set; }

        // Navigation Collections
        [InverseProperty("Personel")]
        public ICollection<BankoKullanici>? BankoKullanicilari { get; set; } = new List<BankoKullanici>();

        public ICollection<KanalPersonel>? KanalPersonelleri { get; set; } = new List<KanalPersonel>();

        [InverseProperty("Personel")]
        public ICollection<PersonelCocuk>? PersonelCocuklari { get; set; } = new List<PersonelCocuk>();

        [InverseProperty("Personel")]
        public ICollection<PersonelYetki>? PersonelYetkileri { get; set; } = new List<PersonelYetki>();

        [InverseProperty("Personel")]
        public ICollection<PersonelHizmet>? PersonelHizmetleri { get; set; } = new List<PersonelHizmet>();

        [InverseProperty("Personel")]
        public ICollection<PersonelEgitim>? PersonelEgitimleri { get; set; } = new List<PersonelEgitim>();

        [InverseProperty("Personel")]
        public ICollection<PersonelImzaYetkisi>? PersonelImzaYetkileri { get; set; } = new List<PersonelImzaYetkisi>();

        [InverseProperty("Personel")]
        public ICollection<PersonelCeza>? PersonelCezalari { get; set; } = new List<PersonelCeza>();

        [InverseProperty("Personel")]
        public ICollection<PersonelEngel>? PersonelEngelleri { get; set; } = new List<PersonelEngel>();

        [InverseProperty("Personel")]
        public ICollection<IzinMazeretTalep>? IzinMazeretTalepleri { get; set; } = new List<IzinMazeretTalep>();

        /// <summary>
        /// Bu personelin sorumlusu olduğu izin onay yetkileri
        /// </summary>
        [InverseProperty("SorumluPersonel")]
        public ICollection<PdksIslemleri.IzinSorumlu>? IzinSorumluluklar { get; set; } = new List<PdksIslemleri.IzinSorumlu>();
    }
}