using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelResponseDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public string Email { get; set; } = string.Empty;

        // Organizasyon bilgileri
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int ServisId { get; set; }
        public string ServisAdi { get; set; } = string.Empty;
        public int UnvanId { get; set; }
        public string UnvanAdi { get; set; } = string.Empty;
        public int AtanmaNedeniId { get; set; }
        public string AtanmaNedeniAdi { get; set; } = string.Empty;
        public int DepartmanHizmetBinasiId { get; set; }
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public int SendikaId { get; set; }
        public string SendikaAdi { get; set; } = string.Empty;

        // İletişim bilgileri
        public int Dahili { get; set; }
        public string? CepTelefonu { get; set; }
        public string? CepTelefonu2 { get; set; }
        public string? EvTelefonu { get; set; }
        public string? Adres { get; set; }
        public string? Semt { get; set; }
        
        // Adres - İl/İlçe
        public int IlId { get; set; }
        public string? IlAdi { get; set; }
        public int IlceId { get; set; }
        public string? IlceAdi { get; set; }

        // Kişisel bilgiler
        public DateTime DogumTarihi { get; set; }
        public int Yas => DateTime.Now.Year - DogumTarihi.Year;
        public Cinsiyet Cinsiyet { get; set; }
        public MedeniDurumu MedeniDurumu { get; set; }
        public KanGrubu KanGrubu { get; set; }
        public EvDurumu EvDurumu { get; set; }
        public int UlasimServis1 { get; set; }
        public int UlasimServis2 { get; set; }
        public int Tabldot { get; set; }

        // ZKTeco PDKS alanları
        public int PersonelKayitNo { get; set; }
        public int KartNo { get; set; }
        public DateTime? KartNoAktiflikTarihi { get; set; }
        public DateTime? KartNoDuzenlenmeTarihi { get; set; }
        public DateTime? KartNoGonderimTarihi { get; set; }
        public IslemBasari KartGonderimIslemBasari { get; set; }

        // Durum bilgileri
        public PersonelTipi PersonelTipi { get; set; }
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; }
        public bool IsOnline { get; set; } = false;

        /// <summary>
        /// Pasif/ayrılış nedeni açıklaması
        /// </summary>
        public string? PasifNedeni { get; set; }

        /// <summary>
        /// Durum değişiklik tarihi (pasif/emekli/istifa vb. olduğu tarih)
        /// </summary>
        public DateTime? DurumDegisiklikTarihi { get; set; }

        // Acil Durum Yakını Bilgileri
        public string? YakinAdSoyad { get; set; }
        public string? YakinlikDerecesi { get; set; }
        public string? YakinTelefonu { get; set; }

        // Kadro ve Göreve Başlama Bilgileri
        public string? KadroDerecesi { get; set; }
        public DateTime? GoreveBaslamaTarihi { get; set; }

        // Finansal Bilgiler
        public string? Iban { get; set; }

        // Ulaşım Bilgileri
        public string? EshotHat { get; set; }
        public string? Durak { get; set; }

        // Taşeron Bilgisi
        public string? TaseronFirma { get; set; }

        // Eğitim bilgileri
        public OgrenimDurumu OgrenimDurumu { get; set; }
        public string? BitirdigiOkul { get; set; }
        public string? BitirdigiBolum { get; set; }
        public int OgrenimSuresi { get; set; }
        public string? Bransi { get; set; }
        public string? EmekliSicilNo { get; set; }
        public SehitYakinligi SehitYakinligi { get; set; }
        
        // Görev/Uzmanlık
        public string? Gorev { get; set; }
        public string? Uzmanlik { get; set; }

        // Eş bilgileri
        public string? EsininAdi { get; set; }
        public EsininIsDurumu EsininIsDurumu { get; set; }
        public string? EsininUnvani { get; set; }
        public string? EsininIsAdresi { get; set; }
        public string? EsininIsSemt { get; set; }
        
        // Eşinin İş Yeri İl/İlçe
        public int? EsininIsIlId { get; set; }
        public string? EsininIsIlAdi { get; set; }
        public int? EsininIsIlceId { get; set; }
        public string? EsininIsIlceAdi { get; set; }

        // JSON Bilgi Alanları
        public string? HizmetBilgisi { get; set; }
        public string? EgitimBilgisi { get; set; }
        public string? ImzaYetkileri { get; set; }
        public string? CezaBilgileri { get; set; }
        public string? EngelBilgileri { get; set; }

        // Resim
        public string? Resim { get; set; }

        // Navigation Collections
        public List<PersonelCocukResponseDto>? Cocuklar { get; set; }
        public List<PersonelHizmetResponseDto>? Hizmetler { get; set; }
        public List<PersonelEgitimResponseDto>? Egitimler { get; set; }
        public List<PersonelImzaYetkisiResponseDto>? ImzaYetkileriDetay { get; set; }
        public List<PersonelCezaResponseDto>? Cezalar { get; set; }
        public List<PersonelEngelResponseDto>? Engeller { get; set; }

        // Audit
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}