using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.Common.Results;
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
        Task<IResult<List<PersonelMesaiListResponseDto>>> GetPersonelMesaiListAsync(PersonelMesaiFilterRequestDto request);

        /// <summary>
        /// Personel bilgilerini getir
        /// </summary>
        Task<IResult<PersonelMesaiBaslikDto>> GetPersonelBaslikBilgiAsync(string tcKimlikNo);
    }
}
