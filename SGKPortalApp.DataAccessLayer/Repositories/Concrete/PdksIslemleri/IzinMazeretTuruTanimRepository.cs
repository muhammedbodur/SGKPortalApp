using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PdksIslemleri
{
    public class IzinMazeretTuruTanimRepository : GenericRepository<IzinMazeretTuruTanim>, IIzinMazeretTuruTanimRepository
    {
        public IzinMazeretTuruTanimRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<IzinMazeretTuruTanim>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Sira)
                .ThenBy(t => t.TuruAdi)
                .ToListAsync();
        }

        public async Task<IzinMazeretTuruTanim?> GetByIdAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IzinMazeretTuruId == id);
        }

        public async Task<IzinMazeretTuruTanim?> GetByKisaKodAsync(string kisaKod)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.KisaKod == kisaKod);
        }
    }
}
