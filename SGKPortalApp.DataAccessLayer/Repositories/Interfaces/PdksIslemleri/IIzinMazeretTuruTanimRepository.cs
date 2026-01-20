using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri
{
    public interface IIzinMazeretTuruTanimRepository : IGenericRepository<IzinMazeretTuruTanim>
    {
        /// <summary>
        /// Tüm aktif izin türlerini getir (sıraya göre)
        /// </summary>
        Task<IEnumerable<IzinMazeretTuruTanim>> GetAllActiveAsync();

        /// <summary>
        /// ID'ye göre izin türü getir
        /// </summary>
        Task<IzinMazeretTuruTanim?> GetByIdAsync(int id);

        /// <summary>
        /// Kısa koda göre izin türü getir
        /// </summary>
        Task<IzinMazeretTuruTanim?> GetByKisaKodAsync(string kisaKod);
    }
}
