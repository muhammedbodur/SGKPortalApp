using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Complex
{
    public class CommonQueryRepository : ICommonQueryRepository
    {
        private readonly SGKDbContext _context;

        public CommonQueryRepository(SGKDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<(int ServisId, string ServisAdi, int PersonelSayisi)>> GetServislerByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            return await (
                from p in _context.Set<Personel>().AsNoTracking()
                join s in _context.Set<Servis>().AsNoTracking() on p.ServisId equals s.ServisId
                where p.DepartmanHizmetBinasi.HizmetBinasiId == hizmetBinasiId
                      && !p.SilindiMi
                      && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif
                      && !s.SilindiMi
                group p by new { p.ServisId, s.ServisAdi } into g
                orderby g.Key.ServisAdi
                select new ValueTuple<int, string, int>(
                    g.Key.ServisId,
                    g.Key.ServisAdi,
                    g.Count()
                )
            ).ToListAsync();
        }
    }
}
