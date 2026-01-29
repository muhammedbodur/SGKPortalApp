using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelAktiflikDurumHareketCreateRequestDto
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        public PersonelAktiflikDurum? OncekiDurum { get; set; }

        [Required(ErrorMessage = "Yeni durum seçimi zorunludur")]
        public PersonelAktiflikDurum YeniDurum { get; set; }

        [Required(ErrorMessage = "Değişiklik tarihi zorunludur")]
        public DateTime DegisiklikTarihi { get; set; }

        public DateTime? GecerlilikBaslangicTarihi { get; set; }

        public DateTime? GecerlilikBitisTarihi { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [StringLength(100, ErrorMessage = "Belge No en fazla 100 karakter olabilir")]
        public string? BelgeNo { get; set; }
    }
}
