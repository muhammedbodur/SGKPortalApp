using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete
{
    public class PersonelCocukRepository : GenericRepository<PersonelCocuk>, IPersonelCocukRepository
    {
        public PersonelCocukRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<List<PersonelCocuk>> GetByPersonelTcKimlikNoAsync(string tcKimlikNo)
        {
            return await _context.PersonelCocuklari
                .Where(pc => pc.PersonelTcKimlikNo == tcKimlikNo)
                .OrderBy(pc => pc.CocukDogumTarihi)
                .ToListAsync();
        }
    }
}
