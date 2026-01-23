using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class BankoCreateRequestDto
    {
        [Required(ErrorMessage = "Departman-Hizmet binası seçilmelidir")]
        public int DepartmanHizmetBinasiId { get; set; }

        [Required(ErrorMessage = "Kat tipi seçilmelidir")]
        public KatTipi KatTipi { get; set; }

        [Required(ErrorMessage = "Banko numarası girilmelidir")]
        [Range(1, 999, ErrorMessage = "Banko numarası 1-999 arasında olmalıdır")]
        public int BankoNo { get; set; }

        [Required(ErrorMessage = "Banko tipi seçilmelidir")]
        public BankoTipi BankoTipi { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? BankoAciklama { get; set; }
    }
}
