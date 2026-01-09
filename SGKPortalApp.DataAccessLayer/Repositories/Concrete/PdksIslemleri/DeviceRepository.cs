using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PdksIslemleri
{
    public class DeviceRepository : GenericRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(SGKDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Device>> GetAllAsync()
        {
            // HizmetBinasi ve Departman navigation property'lerini yükle
            return await _context.Devices
                .Include(d => d.HizmetBinasi)
                    .ThenInclude(hb => hb!.Departman)
                .ToListAsync();
        }

        public async Task<Device?> GetDeviceByIpAsync(string ipAddress)
        {
            return await _context.Devices
                .Include(d => d.HizmetBinasi)
                    .ThenInclude(hb => hb!.Departman)
                .FirstOrDefaultAsync(d => d.IpAddress == ipAddress);
        }
    }
}
