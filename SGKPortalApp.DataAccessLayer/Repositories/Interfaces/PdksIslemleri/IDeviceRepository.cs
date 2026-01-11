using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri
{
    public interface IDeviceRepository : IGenericRepository<Device>
    {
        Task<Device?> GetDeviceByIpAsync(string ipAddress);
        Task<List<Device>> GetActiveDevicesAsync(CancellationToken cancellationToken = default);
        Task<List<Device>> GetAllWithRelationsAsync();
        Task<Device?> GetByIdWithRelationsAsync(int id);
    }
}
