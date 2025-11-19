using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class BankoFormModel
    {
        public int BankoId { get; set; }

        [Required(ErrorMessage = "Hizmet binası seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Hizmet binası seçilmelidir")]
        public int HizmetBinasiId { get; set; }

        [Required(ErrorMessage = "Kat seçilmelidir")]
        public KatTipi KatTipi { get; set; }

        [Required(ErrorMessage = "Banko numarası girilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Banko numarası 1'den büyük olmalıdır")]
        public int BankoNo { get; set; }

        [Required(ErrorMessage = "Banko tipi seçilmelidir")]
        public BankoTipi BankoTipi { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? BankoAciklama { get; set; }
    }
}
