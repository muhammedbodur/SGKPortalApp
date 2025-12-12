using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class DepartmanRepository : GenericRepository<Departman>, IDepartmanRepository
    {
        public DepartmanRepository(SGKDbContext context) : base(context)
        {
        }

        public override async Task<Departman?> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(d => d.Personeller.Where(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif))
                .FirstOrDefaultAsync(d => d.DepartmanId == (int)id && !d.SilindiMi);
        }

        public async Task<Departman?> GetByDepartmanAdiAsync(string departmanAdi)
        {
            if (string.IsNullOrWhiteSpace(departmanAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DepartmanAdi == departmanAdi && !d.SilindiMi);
        }

        public async Task<IEnumerable<Departman>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(d => d.Personeller.Where(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif))
                .Where(d => !d.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(d => !d.SilindiMi)
                .Select(d => new ValueTuple<int, string>(d.DepartmanId, d.DepartmanAdi))
                .ToListAsync();
        }

        public async Task<int> GetPersonelCountAsync(int departmanId)
        {
            return await _context.Set<Personel>()
                .AsNoTracking()
                .CountAsync(p => p.DepartmanId == departmanId && !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);
        }
    }
}