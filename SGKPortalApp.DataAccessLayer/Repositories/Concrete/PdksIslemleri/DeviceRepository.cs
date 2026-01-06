using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PdksIslemleri
{
    public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Device?> GetDeviceByIpAsync(string ipAddress)
        {
            return await _context.Devices
                .FirstOrDefaultAsync(d => d.IpAddress == ipAddress);
        }
    }
}
