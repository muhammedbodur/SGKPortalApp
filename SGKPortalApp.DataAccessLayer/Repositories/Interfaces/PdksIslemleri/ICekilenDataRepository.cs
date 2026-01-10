using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri
{
    public interface ICekilenDataRepository : IGenericRepository<CekilenData>
    {
        Task<List<CekilenData>> GetByEnrollNumberAsync(string enrollNumber);
        Task<List<CekilenData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<CekilenData>> GetByDeviceIpAsync(string deviceIp);
        Task<bool> ExistsAsync(string enrollNumber, DateTime date);
    }
}
