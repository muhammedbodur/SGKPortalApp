using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels
{
    public class PersonelFormModel
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sicil No zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Sicil No giriniz")]
        public int SicilNo { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Ad Soyad 3-200 karakter arasında olmalıdır")]
        public string AdSoyad { get; set; } = string.Empty;

        public string? NickName { get; set; }
        
        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        public int DepartmanId { get; set; }
        public int ServisId { get; set; }
        public int UnvanId { get; set; }
        public int HizmetBinasiId { get; set; }
        public int Dahili { get; set; }
        public string? CepTelefonu { get; set; }
        public string? CepTelefonu2 { get; set; }
        public string? EvTelefonu { get; set; }
        public string? Adres { get; set; }
        public string? Semt { get; set; }
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public int AtanmaNedeniId { get; set; }
        public DateTime DogumTarihi { get; set; } = DateTime.Now.AddYears(-30);
        public Cinsiyet Cinsiyet { get; set; }
        public MedeniDurumu MedeniDurumu { get; set; }
        public KanGrubu KanGrubu { get; set; }
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; } = PersonelAktiflikDurum.Aktif;
        public PersonelTipi PersonelTipi { get; set; }
        public OgrenimDurumu OgrenimDurumu { get; set; }
        public string? BitirdigiOkul { get; set; }
        public string? BitirdigiBolum { get; set; }
        public string? EmekliSicilNo { get; set; }
        public string? Resim { get; set; }
        public string? EsininAdi { get; set; }
        public EsininIsDurumu EsininIsDurumu { get; set; }
        public string? EsininUnvani { get; set; }
        public string? EsininIsAdresi { get; set; }
    }

    // Dinamik Liste Modelleri
    public class CocukModel
    {
        public string Isim { get; set; } = string.Empty;
        public DateTime? DogumTarihi { get; set; }
        public string? Egitim { get; set; }
    }

    public class HizmetModel
    {
        public int DepartmanId { get; set; }
        public string? Departman { get; set; }
        public int ServisId { get; set; }
        public string? Servis { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? AyrilmaTarihi { get; set; }
        public string? Sebep { get; set; }
    }

    public class EgitimModel
    {
        public string? EgitimAdi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }

    public class YetkiModel
    {
        public int DepartmanId { get; set; }
        public int ServisId { get; set; }
        public string? GorevDegisimSebebi { get; set; }
        public DateTime? ImzaYetkisiBaslamaTarihi { get; set; }
        public DateTime? ImzaYetkisiBitisTarihi { get; set; }
    }

    public class CezaModel
    {
        public string? CezaSebebi { get; set; }
        public string? AltBendi { get; set; }
        public DateTime? CezaTarihi { get; set; }
    }

    public class EngelModel
    {
        public EngelDerecesi EngelDerecesi { get; set; }
        public string? EngelNedeni1 { get; set; }
        public string? EngelNedeni2 { get; set; }
        public string? EngelNedeni3 { get; set; }
    }

    public class Step1Model
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Ad Soyad 3-200 karakter arasında olmalıdır")]
        public string AdSoyad { get; set; } = string.Empty;
    }
}
