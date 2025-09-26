using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri
{
    public interface IKioskIslemGrupRepository : IGenericRepository<KioskIslemGrup>
    {
        // Kiosk grubu bazında işlem gruplarını listeler
        Task<IEnumerable<KioskIslemGrup>> GetByKioskGrupAsync(int kioskGrupId);

        // Hizmet binası bazında işlem gruplarını listeler
        Task<IEnumerable<KioskIslemGrup>> GetByHizmetBinasiAsync(int hizmetBinasiId);

        // Tarih aralığına göre işlem gruplarını listeler
        Task<IEnumerable<KioskIslemGrup>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // İşlem grubunu detaylı getirir
        Task<KioskIslemGrup?> GetWithDetailsAsync(int kioskIslemGrupId);

        // Tüm işlem gruplarını detaylı listeler
        Task<IEnumerable<KioskIslemGrup>> GetAllWithDetailsAsync();

        // Aktif işlem gruplarını listeler
        Task<IEnumerable<KioskIslemGrup>> GetActiveAsync();
    }
}