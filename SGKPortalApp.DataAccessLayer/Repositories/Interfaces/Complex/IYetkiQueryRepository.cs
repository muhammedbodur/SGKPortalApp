using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex
{
    public interface IYetkiQueryRepository
    {
        /// <summary>
        /// Belirtilen controller için tam hiyerarşik yolu (FullPath) döndürür
        /// Recursive CTE kullanarak root'tan leaf'e kadar tüm hiyerarşiyi oluşturur
        /// </summary>
        /// <param name="modulControllerId">Controller ID</param>
        /// <returns>Tuple: (ModulControllerId, ModulControllerAdi, Level, FullPath)</returns>
        Task<List<(int ModulControllerId, string ModulControllerAdi, int Level, string FullPath)>> GetControllerHierarchyPathAsync(int modulControllerId);

        /// <summary>
        /// Belirtilen modüle ait tüm controller'ların hiyerarşik yollarını döndürür
        /// </summary>
        /// <param name="modulId">Modül ID</param>
        /// <returns>Tuple: (ModulControllerId, ModulControllerAdi, Level, FullPath)</returns>
        Task<List<(int ModulControllerId, string ModulControllerAdi, int Level, string FullPath)>> GetModulControllersWithHierarchyAsync(int modulId);
    }
}
