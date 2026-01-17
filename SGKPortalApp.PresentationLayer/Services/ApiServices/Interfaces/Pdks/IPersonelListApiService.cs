using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks
{
    public interface IPersonelListApiService
    {
        Task<ServiceResult<List<DepartmanDto>>> GetDepartmanListeAsync();
        Task<ServiceResult<List<ServisDto>>> GetServisListeAsync();
        Task<ServiceResult<List<PersonelListResponseDto>>> GetPersonelListeAsync(object request);
    }
}
