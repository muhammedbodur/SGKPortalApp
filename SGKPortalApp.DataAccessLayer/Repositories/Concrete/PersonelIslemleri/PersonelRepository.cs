using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
    {
        public PersonelRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Personel?> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TcKimlikNo == tcKimlikNo && !p.SilindiMi);
        }

        public async Task<Personel?> GetBySicilNoAsync(int sicilNo)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.SicilNo == sicilNo && !p.SilindiMi);
        }

        public async Task<IEnumerable<Personel>> GetByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.DepartmanId == departmanId && !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Personel>> GetByUnvanAsync(int unvanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.UnvanId == unvanId && !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Personel>> GetActivePersonelAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string AdSoyad)>> GetPersonelDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.SilindiMi)
                .Select(p => new ValueTuple<int, string>(p.SicilNo, p.AdSoyad))
                .ToListAsync();
        }

        public async Task<Personel?> GetByTcKimlikNoWithDetailsAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            return await _dbSet
                .Include(p => p.Departman)
                .Include(p => p.Servis)
                .Include(p => p.Unvan)
                .Include(p => p.Il)
                .Include(p => p.Ilce)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TcKimlikNo == tcKimlikNo && !p.SilindiMi);
        }

        public async Task<IEnumerable<Personel>> GetByServisAsync(int servisId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.ServisId == servisId && !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Personel>> GetActiveAsync()
        {
            return await _dbSet
                .Include(p => p.Departman)
                .Include(p => p.Servis)
                .Include(p => p.Unvan)
                .AsNoTracking()
                .Where(p => !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Personel>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(p => p.Departman)
                .Include(p => p.Servis)
                .Include(p => p.Unvan)
                .AsNoTracking()
                .Where(p => !p.SilindiMi)
                .OrderBy(p => p.AdSoyad)
                .ToListAsync();
        }

        public async Task<PagedResponseDto<PersonelListResponseDto>> GetPagedAsync(PersonelFilterRequestDto filter)
        {
            var query = _dbSet
                .Include(p => p.Departman)
                .Include(p => p.Servis)
                .Include(p => p.Unvan)
                .Where(p => !p.SilindiMi)
                .AsQueryable();

            // Filtreleme
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(p =>
                    p.AdSoyad.Contains(filter.SearchTerm) ||
                    p.TcKimlikNo.Contains(filter.SearchTerm) ||
                    p.Email.Contains(filter.SearchTerm));
            }

            if (filter.DepartmanId.HasValue && filter.DepartmanId > 0)
            {
                query = query.Where(p => p.DepartmanId == filter.DepartmanId.Value);
            }

            if (filter.ServisId.HasValue && filter.ServisId > 0)
            {
                query = query.Where(p => p.ServisId == filter.ServisId.Value);
            }

            if (filter.AktiflikDurum.HasValue)
            {
                query = query.Where(p => p.PersonelAktiflikDurum == filter.AktiflikDurum.Value);
            }

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Sıralama
            query = filter.SortBy?.ToLower() switch
            {
                "adsoyad" => filter.SortDescending ? query.OrderByDescending(p => p.AdSoyad) : query.OrderBy(p => p.AdSoyad),
                "sicilno" => filter.SortDescending ? query.OrderByDescending(p => p.SicilNo) : query.OrderBy(p => p.SicilNo),
                "departman" => filter.SortDescending ? query.OrderByDescending(p => p.Departman!.DepartmanAdi) : query.OrderBy(p => p.Departman!.DepartmanAdi),
                _ => query.OrderBy(p => p.AdSoyad)
            };

            // Sayfalama
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new PersonelListResponseDto
                {
                    TcKimlikNo = p.TcKimlikNo,
                    SicilNo = p.SicilNo,
                    AdSoyad = p.AdSoyad,
                    Email = p.Email,
                    DepartmanAdi = p.Departman != null ? p.Departman.DepartmanAdi : "",
                    ServisAdi = p.Servis != null ? p.Servis.ServisAdi : "",
                    UnvanAdi = p.Unvan != null ? p.Unvan.UnvanAdi : "",
                    Dahili = p.Dahili,
                    CepTelefonu = p.CepTelefonu,
                    PersonelAktiflikDurum = p.PersonelAktiflikDurum.ToString()
                })
                .ToListAsync();

            return new PagedResponseDto<PersonelListResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
    }
}