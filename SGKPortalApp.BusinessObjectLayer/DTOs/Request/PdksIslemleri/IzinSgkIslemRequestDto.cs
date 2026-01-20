using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// İzin SGK işlem Request DTO
    /// SGK sistemine işleme için
    /// </summary>
    public class IzinSgkIslemRequestDto
    {
        /// <summary>
        /// İzin/Mazeret Talep ID
        /// </summary>
        [Required(ErrorMessage = "Talep ID zorunludur")]
        public int IzinMazeretTalepId { get; set; }

        /// <summary>
        /// İşlem yapılacak mı? (true=işle, false=geri al)
        /// </summary>
        public bool Isle { get; set; }

        /// <summary>
        /// İşlem notları
        /// </summary>
        [StringLength(1000, ErrorMessage = "Notlar en fazla 1000 karakter olabilir")]
        public string? Notlar { get; set; }
    }
}
