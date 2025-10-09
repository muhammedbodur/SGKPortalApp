using AutoMapper;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class IlService : IIlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IlService> _logger;

        public IlService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<IlService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<IlResponseDto>>> GetAllAsync()
        {
            try
            {
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var iller = await ilRepo.GetAllAsync();
                
                var ilDtos = new List<IlResponseDto>();
                foreach (var il in iller)
                {
                    var ilceCount = await ilRepo.GetIlceCountAsync(il.IlId);
                    var ilDto = _mapper.Map<IlResponseDto>(il);
                    ilDto.IlceSayisi = ilceCount;
                    ilDtos.Add(ilDto);
                }

                return ApiResponseDto<List<IlResponseDto>>
                    .SuccessResult(ilDtos, "İller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl listesi getirilirken hata oluştu");
                return ApiResponseDto<List<IlResponseDto>>
                    .ErrorResult("İller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("Geçersiz il ID");

                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var il = await ilRepo.GetByIdAsync(id);

                if (il == null)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("İl bulunamadı");

                var ilDto = _mapper.Map<IlResponseDto>(il);
                ilDto.IlceSayisi = await ilRepo.GetIlceCountAsync(id);

                return ApiResponseDto<IlResponseDto>
                    .SuccessResult(ilDto, "İl başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl getirilirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IlResponseDto>
                    .ErrorResult("İl getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlResponseDto>> CreateAsync(IlCreateRequestDto request)
        {
            try
            {
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var existingIl = await ilRepo.GetByIlAdiAsync(request.IlAdi);

                if (existingIl != null)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("Bu isimde bir il zaten mevcut");

                var il = _mapper.Map<Il>(request);
                await ilRepo.AddAsync(il);
                await _unitOfWork.SaveChangesAsync();

                var ilDto = _mapper.Map<IlResponseDto>(il);
                ilDto.IlceSayisi = 0;
                _logger.LogInformation("Yeni il oluşturuldu. ID: {Id}", il.IlId);

                return ApiResponseDto<IlResponseDto>
                    .SuccessResult(ilDto, "İl başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl oluşturulurken hata oluştu");
                return ApiResponseDto<IlResponseDto>
                    .ErrorResult("İl oluşturulurken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlResponseDto>> UpdateAsync(int id, IlUpdateRequestDto request)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("Geçersiz il ID");

                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var il = await ilRepo.GetByIdAsync(id);

                if (il == null)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("İl bulunamadı");

                var existingIl = await ilRepo.GetByIlAdiAsync(request.IlAdi);
                if (existingIl != null && existingIl.IlId != id)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("Bu isimde başka bir il zaten mevcut");

                il.IlAdi = request.IlAdi;
                il.DuzenlenmeTarihi = DateTime.Now;

                ilRepo.Update(il);
                await _unitOfWork.SaveChangesAsync();

                var ilDto = _mapper.Map<IlResponseDto>(il);
                ilDto.IlceSayisi = await ilRepo.GetIlceCountAsync(id);
                _logger.LogInformation("İl güncellendi. ID: {Id}", id);

                return ApiResponseDto<IlResponseDto>
                    .SuccessResult(ilDto, "İl başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl güncellenirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<IlResponseDto>
                    .ErrorResult("İl güncellenirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponseDto<bool>
                        .ErrorResult("Geçersiz il ID");

                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var il = await ilRepo.GetByIdAsync(id);

                if (il == null)
                    return ApiResponseDto<bool>
                        .ErrorResult("İl bulunamadı");

                var ilceCount = await ilRepo.GetIlceCountAsync(id);
                if (ilceCount > 0)
                    return ApiResponseDto<bool>
                        .ErrorResult($"Bu ile ait {ilceCount} ilçe bulunmaktadır. Önce ilçeleri silmelisiniz.");

                ilRepo.Delete(il);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("İl silindi. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .SuccessResult(true, "İl başarıyla silindi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl silinirken hata oluştu. ID: {Id}", id);
                return ApiResponseDto<bool>
                    .ErrorResult("İl silinirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<IlResponseDto>> GetByNameAsync(string ilAdi)
        {
            try
            {
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var il = await ilRepo.GetByIlAdiAsync(ilAdi);

                if (il == null)
                    return ApiResponseDto<IlResponseDto>
                        .ErrorResult("İl bulunamadı");

                var ilDto = _mapper.Map<IlResponseDto>(il);
                ilDto.IlceSayisi = await ilRepo.GetIlceCountAsync(il.IlId);

                return ApiResponseDto<IlResponseDto>
                    .SuccessResult(ilDto, "İl başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl getirilirken hata oluştu");
                return ApiResponseDto<IlResponseDto>
                    .ErrorResult("İl getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<PagedResponseDto<IlResponseDto>>> GetPagedAsync(IlFilterRequestDto filter)
        {
            try
            {
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var iller = await ilRepo.GetAllAsync();

                // Filtreleme
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    iller = iller.Where(i => i.IlAdi.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                // Sıralama
                iller = filter.OrderBy.ToLower() switch
                {
                    "iladi" => filter.OrderDirection == "desc" 
                        ? iller.OrderByDescending(i => i.IlAdi) 
                        : iller.OrderBy(i => i.IlAdi),
                    "eklenmetarihi" => filter.OrderDirection == "desc" 
                        ? iller.OrderByDescending(i => i.EklenmeTarihi) 
                        : iller.OrderBy(i => i.EklenmeTarihi),
                    _ => iller.OrderBy(i => i.IlAdi)
                };

                var totalCount = iller.Count();
                var pagedData = iller
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var ilDtos = new List<IlResponseDto>();
                foreach (var il in pagedData)
                {
                    var ilceCount = await ilRepo.GetIlceCountAsync(il.IlId);
                    var ilDto = _mapper.Map<IlResponseDto>(il);
                    ilDto.IlceSayisi = ilceCount;
                    ilDtos.Add(ilDto);
                }

                var pagedResult = new PagedResponseDto<IlResponseDto>
                {
                    Items = ilDtos,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                };

                return ApiResponseDto<PagedResponseDto<IlResponseDto>>
                    .SuccessResult(pagedResult, "İller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sayfalı il listesi getirilirken hata oluştu");
                return ApiResponseDto<PagedResponseDto<IlResponseDto>>
                    .ErrorResult("İller getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<int>> GetIlceCountAsync(int ilId)
        {
            try
            {
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var count = await ilRepo.GetIlceCountAsync(ilId);

                return ApiResponseDto<int>
                    .SuccessResult(count, "İlçe sayısı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçe sayısı getirilirken hata oluştu");
                return ApiResponseDto<int>
                    .ErrorResult("İlçe sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync()
        {
            try
            {
                var ilRepo = _unitOfWork.GetRepository<IIlRepository>();
                var dropdown = await ilRepo.GetDropdownAsync();

                return ApiResponseDto<List<(int Id, string Ad)>>
                    .SuccessResult(dropdown.ToList(), "İl dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İl dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<(int Id, string Ad)>>
                    .ErrorResult("İl dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
