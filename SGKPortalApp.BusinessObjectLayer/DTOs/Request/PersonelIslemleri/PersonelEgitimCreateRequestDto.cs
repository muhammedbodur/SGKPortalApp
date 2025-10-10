using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelEgitimCreateRequestDto
    {
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Eğitim adı zorunludur")]
        [StringLength(200, ErrorMessage = "Eğitim adı en fazla 200 karakter olabilir")]
        public string EgitimAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Eğitim başlangıç tarihi zorunludur")]
        public DateTime EgitimBaslangicTarihi { get; set; }

        public DateTime? EgitimBitisTarihi { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }
    }
}
