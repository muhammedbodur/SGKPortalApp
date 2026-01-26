using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
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

        public async Task<Personel?> GetByPersonelKayitNoAsync(int personelKayitNo)
        {
            return await _dbSet
                .Include(p => p.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PersonelKayitNo == personelKayitNo && !p.SilindiMi);
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
                .Include(p => p.AtanmaNedeni)
                .Include(p => p.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .Include(p => p.Sendika)
                .Include(p => p.Il)
                .Include(p => p.Ilce)
                .Include(p => p.EsininIsIl)
                .Include(p => p.EsininIsIlce)
                .Include(p => p.PersonelCocuklari)
                .Include(p => p.PersonelHizmetleri)
                    .ThenInclude(h => h.Departman)
                .Include(p => p.PersonelHizmetleri)
                    .ThenInclude(h => h.Servis)
                .Include(p => p.PersonelEgitimleri)
                .Include(p => p.PersonelImzaYetkileri)
                    .ThenInclude(y => y.Departman)
                .Include(p => p.PersonelImzaYetkileri)
                    .ThenInclude(y => y.Servis)
                .Include(p => p.PersonelCezalari)
                .Include(p => p.PersonelEngelleri)
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

        public async Task<IEnumerable<Personel>> GetByHizmetBinasiIdAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(p => p.Departman)
                .Include(p => p.Servis)
                .Include(p => p.Unvan)
                .Include(p => p.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .AsNoTracking()
                .Where(p => p.DepartmanHizmetBinasi.HizmetBinasiId == hizmetBinasiId &&
                           !p.SilindiMi &&
                           p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif)
                .OrderBy(p => p.AdSoyad)
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
                .Include(p => p.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
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
                .Include(p => p.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .Where(p => !p.SilindiMi)
                .AsQueryable();

            // Filtreleme
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var trimmedTerm = filter.SearchTerm.Trim();
                var isNumeric = int.TryParse(trimmedTerm, out var sicilNoSearch);

                query = query.Where(p =>
                    p.AdSoyad.Contains(trimmedTerm) ||
                    p.TcKimlikNo.Contains(trimmedTerm) ||
                    p.Email.Contains(trimmedTerm) ||
                    (isNumeric && p.SicilNo == sicilNoSearch));
            }

            if (filter.DepartmanId.HasValue && filter.DepartmanId > 0)
            {
                query = query.Where(p => p.DepartmanId == filter.DepartmanId.Value);
            }

            if (filter.ServisId.HasValue && filter.ServisId > 0)
            {
                query = query.Where(p => p.ServisId == filter.ServisId.Value);
            }

            if (filter.UnvanId.HasValue && filter.UnvanId > 0)
            {
                query = query.Where(p => p.UnvanId == filter.UnvanId.Value);
            }

            if (filter.DepartmanHizmetBinasiId.HasValue && filter.DepartmanHizmetBinasiId > 0)
            {
                query = query.Where(p => p.DepartmanHizmetBinasiId == filter.DepartmanHizmetBinasiId.Value);
            }

            if (filter.AktiflikDurum.HasValue)
            {
                query = query.Where(p => p.PersonelAktiflikDurum == filter.AktiflikDurum.Value);
            }

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Sıralama - TÜM ALANLAR İÇİN
            query = filter.SortBy?.ToLower() switch
            {
                "tckimlikno" => filter.SortDescending
                    ? query.OrderByDescending(p => p.TcKimlikNo)
                    : query.OrderBy(p => p.TcKimlikNo),

                "sicilno" => filter.SortDescending
                    ? query.OrderByDescending(p => p.SicilNo)
                    : query.OrderBy(p => p.SicilNo),

                "adsoyad" => filter.SortDescending
                    ? query.OrderByDescending(p => p.AdSoyad)
                    : query.OrderBy(p => p.AdSoyad),

                "departmanadi" => filter.SortDescending
                    ? query.OrderByDescending(p => p.Departman != null ? p.Departman.DepartmanAdi : "")
                    : query.OrderBy(p => p.Departman != null ? p.Departman.DepartmanAdi : ""),

                "servisadi" => filter.SortDescending
                    ? query.OrderByDescending(p => p.Servis != null ? p.Servis.ServisAdi : "")
                    : query.OrderBy(p => p.Servis != null ? p.Servis.ServisAdi : ""),

                "hizmetbinasiadi" => filter.SortDescending
                    ? query.OrderByDescending(p => p.DepartmanHizmetBinasi != null && p.DepartmanHizmetBinasi.HizmetBinasi != null ? p.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : "")
                    : query.OrderBy(p => p.DepartmanHizmetBinasi != null && p.DepartmanHizmetBinasi.HizmetBinasi != null ? p.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : ""),

                "unvanadi" => filter.SortDescending
                    ? query.OrderByDescending(p => p.Unvan != null ? p.Unvan.UnvanAdi : "")
                    : query.OrderBy(p => p.Unvan != null ? p.Unvan.UnvanAdi : ""),

                "dahili" => filter.SortDescending
                    ? query.OrderByDescending(p => p.Dahili)
                    : query.OrderBy(p => p.Dahili),

                "eklenmetarihi" => filter.SortDescending
                    ? query.OrderByDescending(p => p.EklenmeTarihi)
                    : query.OrderBy(p => p.EklenmeTarihi),

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
                    HizmetBinasiAdi = p.DepartmanHizmetBinasi != null && p.DepartmanHizmetBinasi.HizmetBinasi != null ? p.DepartmanHizmetBinasi.HizmetBinasi.HizmetBinasiAdi : "",
                    Dahili = p.Dahili,
                    CepTelefonu = p.CepTelefonu,
                    Resim = p.Resim ?? "",
                    PersonelAktiflikDurum = p.PersonelAktiflikDurum,
                    EklenmeTarihi = p.EklenmeTarihi
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

        public async Task<string> GetNextAvailableEnrollNumberAsync()
        {
            // Personeller için EnrollNumber aralığı: 1-59999
            var maxEnrollNumber = await _dbSet
                .Where(p => !p.SilindiMi)
                .Select(p => p.PersonelKayitNo)
                .OrderByDescending(p => p)
                .FirstOrDefaultAsync();

            // Eğer hiç personel yoksa 1'den başla, varsa max+1
            var nextNumber = maxEnrollNumber == 0 ? 1 : maxEnrollNumber + 1;

            // 59999'u geçmemeli
            if (nextNumber > 59999)
                throw new InvalidOperationException("Personel EnrollNumber kapasitesi doldu (max: 59999)");

            return nextNumber.ToString();
        }
    }
}