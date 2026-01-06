using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri
{
    public interface IDeviceRepository : IGenericRepository<Device>
    {
        Task<Device?> GetDeviceByIpAsync(string ipAddress);
    }
}
