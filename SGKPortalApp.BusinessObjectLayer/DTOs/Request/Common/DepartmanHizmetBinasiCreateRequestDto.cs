using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class DepartmanHizmetBinasiCreateRequestDto
    {
        [Required(ErrorMessage = "Departman seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Departman seçilmelidir")]
        public int DepartmanId { get; set; }

        [Required(ErrorMessage = "Hizmet binası seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Hizmet binası seçilmelidir")]
        public int HizmetBinasiId { get; set; }

        public bool AnaBina { get; set; } = false;
    }
}
