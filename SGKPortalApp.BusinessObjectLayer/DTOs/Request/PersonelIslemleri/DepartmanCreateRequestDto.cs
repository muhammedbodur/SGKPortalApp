using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class DepartmanCreateRequestDto
    {
        [Required(ErrorMessage = "Departman adı zorunludur")]
        [StringLength(150, ErrorMessage = "Departman adı en fazla 150 karakter olabilir")]
        public string DepartmanAdi { get; set; } = string.Empty;
    }
}