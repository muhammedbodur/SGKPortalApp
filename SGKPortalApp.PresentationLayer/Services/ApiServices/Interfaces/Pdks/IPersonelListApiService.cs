using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks
{
    public interface IPersonelListApiService
    {
        /// <summary>
        /// Kullanıcının yetkili olduğu departman listesi (claims'e göre filtrelenmiş)
        /// </summary>
        Task<ServiceResult<List<DepartmanDto>>> GetDepartmanListeAsync();

        /// <summary>
        /// Tüm departman listesi (modal/form için)
        /// </summary>
        Task<ServiceResult<List<DepartmanDto>>> GetAllDepartmanListeAsync();

        /// <summary>
        /// Kullanıcının yetkili olduğu servis listesi (claims'e göre filtrelenmiş)
        /// </summary>
        Task<ServiceResult<List<ServisDto>>> GetServisListeAsync();

        /// <summary>
        /// Tüm servis listesi (modal/form için)
        /// </summary>
        Task<ServiceResult<List<ServisDto>>> GetAllServisListeAsync();

        Task<ServiceResult<List<PersonelListResponseDto>>> GetPersonelListeAsync(object request);
    }
}
