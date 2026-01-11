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

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public int DepartmanId { get; set; }
        public int ServisId { get; set; }
        public int UnvanId { get; set; }
        public int HizmetBinasiId { get; set; }

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