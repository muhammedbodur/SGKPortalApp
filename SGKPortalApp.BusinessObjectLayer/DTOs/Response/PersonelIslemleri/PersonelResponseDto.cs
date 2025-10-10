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

        // İletişim bilgileri
        public int Dahili { get; set; }
        public string? CepTelefonu { get; set; }
        public string? CepTelefonu2 { get; set; }
        public string? EvTelefonu { get; set; }
        public string? Adres { get; set; }
        public string? Semt { get; set; }

        // Kişisel bilgiler
        public DateTime DogumTarihi { get; set; }
        public int Yas => DateTime.Now.Year - DogumTarihi.Year;
        public Cinsiyet Cinsiyet { get; set; }
        public MedeniDurumu MedeniDurumu { get; set; }
        public KanGrubu KanGrubu { get; set; }

        // Durum bilgileri
        public PersonelTipi PersonelTipi { get; set; }
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; }

        // Eğitim bilgileri
        public OgrenimDurumu OgrenimDurumu { get; set; }
        public string? BitirdigiOkul { get; set; }
        public string? BitirdigiBolum { get; set; }

        // Resim
        public string? Resim { get; set; }

        // Audit
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}