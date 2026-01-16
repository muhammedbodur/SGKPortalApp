using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    /// <summary>
    /// Personel Mesai Service Interface
    /// </summary>
    public interface IPersonelMesaiService
    {
        /// <summary>
        /// Personel mesai kayıtlarını filtrele
        /// </summary>
        Task<ApiResponseDto<List<PersonelMesaiListResponseDto>>> GetPersonelMesaiListAsync(PersonelMesaiFilterRequestDto request);

        /// <summary>
        /// Personel bilgilerini getir
        /// </summary>
        Task<ApiResponseDto<PersonelMesaiBaslikDto>> GetPersonelBaslikBilgiAsync(string tcKimlikNo);
    }
}
